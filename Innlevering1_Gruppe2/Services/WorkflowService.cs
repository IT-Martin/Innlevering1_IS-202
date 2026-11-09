using Reachout.DataAccess;
using Reachout.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Reachout.Services;
public class WorkflowService
{
    private readonly ApplicationDbContext context;
    public WorkflowService(ApplicationDbContext context)
    {
        this.context = context;
    }

    // Låsen hindrer at to samtidige forespørsler reserverer samme ressurs.
    private async Task<string?> ChangeAsync(Func<Task<string?>> action)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        await context.Database.ExecuteSqlRawAsync("SELECT Id FROM AppWriteLock WHERE Id=1 FOR UPDATE");
        var error = await action();
        if (error != null)
        {
            context.ChangeTracker.Clear();
            return error;
        }

        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        return null;
    }

    public Task<string?> RequestAsync(int resourceId, int needId, int accountId, DateTime start, DateTime end, string message)
    {
        return ChangeAsync(async () =>
        {
            var resource = await context.Resources.SingleOrDefaultAsync(resource => resource.resource_id == resourceId && !resource.archived);
            var need = await context.ResourceNeeds.SingleOrDefaultAsync(need => need.resource_need_id == needId && need.public_actor_account_id == accountId && !need.archived && need.status != "Løst");
            if (resource == null || need == null || resource.availability_status == "Ikke tilgjengelig")
            {
                return "Velg en tilgjengelig ressurs og ett av dine åpne behov.";
            }

            if (need.deadline.HasValue && need.deadline < DateTime.Now)
            {
                return "Svarfristen for behovet er utløpt.";
            }

            if (end <= start || start <= DateTime.Now)
            {
                return "Velg et fremtidig tidsrom med slutt etter start.";
            }

            if (resource.resource_type_id != need.resource_type_id || start < need.required_start || end > need.required_end)
            {
                return "Velg samme kategori og et tidsrom innenfor behovets periode.";
            }

            if (await context.ResourceMatches.AnyAsync(match => match.resource_need_id == needId))
            {
                return "Behovet har allerede en ressurskobling. Åpne den eksisterende forespørselen. Et separat ressursbehov må registreres som et nytt behov.";
            }

            if (!await context.AvailabilityPeriods.AnyAsync(period => period.resource_id == resourceId && period.available_from <= start && period.available_to >= end))
            {
                return "Tidsrommet er utenfor ressursens tilgjengelighet.";
            }

            if (await context.Reservations.AnyAsync(reservation => reservation.ResourceMatch.resource_id == resourceId && (reservation.reservation_status == "Godtatt" || reservation.public_actor_account_id == accountId && reservation.reservation_status == "Venter på svar") && reservation.requested_start < end && reservation.requested_end > start))
            {
                return "Tidsrommet overlapper en reservasjon eller din eksisterende forespørsel.";
            }

            var match = new ResourceMatch
            {
                resource_id = resourceId,
                resource_need_id = needId,
                public_actor_account_id = accountId
            };
            context.Reservations.Add(new Reservation { ResourceMatch = match, public_actor_account_id = accountId, requested_start = start, requested_end = end, message = message });
            return null;
        });
    }

    public Task<string?> RespondAsync(int id, int accountId, bool supplier, string status)
    {
        return ChangeAsync(async () =>
        {
            var reservation = await context.Reservations.Include(reservation => reservation.ResourceMatch).ThenInclude(match => match.Resource).Include(reservation => reservation.ResourceMatch.ResourceNeed).SingleOrDefaultAsync(reservation => reservation.reservation_id == id);
            if (reservation == null)
            {
                return "Forespørselen finnes ikke.";
            }

            var match = reservation.ResourceMatch;
            if (supplier)
            {
                if (match.Resource.account_id != accountId || reservation.reservation_status != "Venter på svar" || status is not ("Godtatt" or "Avslått"))
                {
                    return "Ingen gyldig endring.";
                }

                if (status == "Godtatt")
                {
                    if (match.Resource.archived || match.Resource.availability_status == "Ikke tilgjengelig" || match.ResourceNeed.archived || match.ResourceNeed.status == "Løst" || reservation.requested_start <= DateTime.Now)
                    {
                        return "Ressursen, behovet eller tidsrommet er ikke lenger tilgjengelig.";
                    }

                    if (!await context.AvailabilityPeriods.AnyAsync(period => period.resource_id == match.resource_id && period.available_from <= reservation.requested_start && period.available_to >= reservation.requested_end))
                    {
                        return "Tilgjengeligheten dekker ikke forespørselen.";
                    }

                    if (await context.Reservations.AnyAsync(other => other.reservation_id != id && other.ResourceMatch.resource_id == match.resource_id && other.reservation_status == "Godtatt" && other.requested_start < reservation.requested_end && other.requested_end > reservation.requested_start))
                    {
                        return "Ressursen er allerede reservert i tidsrommet.";
                    }
                }
            }
            else if (reservation.public_actor_account_id != accountId || reservation.reservation_status is not ("Venter på svar" or "Godtatt") || status is not ("Avbrutt" or "Fullført") || status == "Fullført" && reservation.reservation_status != "Godtatt")
            {
                return "Ingen gyldig endring.";
            }

            reservation.reservation_status = status;
            match.match_status = status;
            match.assessed_at = DateTime.UtcNow;
            await context.SaveChangesAsync();
            var assigned = await context.Reservations.AnyAsync(other => other.ResourceMatch.resource_need_id == match.resource_need_id && other.reservation_status == "Godtatt");
            if (assigned)
            {
                match.ResourceNeed.status = "Tildelt";
            }
            else if (match.ResourceNeed.status == "Tildelt")
            {
                match.ResourceNeed.status = "Vurdert";
            }

            return null;
        });
    }

    public Task<string?> NeedStatusAsync(int id, int accountId, string status)
    {
        return ChangeAsync(async () =>
        {
            var need = await context.ResourceNeeds.SingleOrDefaultAsync(need => need.resource_need_id == id && need.public_actor_account_id == accountId && !need.archived);
            if (need == null || status is not ("Ny" or "Vurdert" or "Løst"))
            {
                return "Velg et av dine behov og en gyldig status.";
            }

            if (await context.Reservations.AnyAsync(reservation => reservation.ResourceMatch.resource_need_id == id && reservation.reservation_status == "Godtatt"))
            {
                return "Avklar bekreftede reservasjoner før behovet avsluttes eller endrer behandlingsstatus.";
            }

            if (status == "Løst")
            {
                var waiting = await context.Reservations.Include(reservation => reservation.ResourceMatch).Where(reservation => reservation.ResourceMatch.resource_need_id == id && reservation.reservation_status == "Venter på svar").ToListAsync();
                foreach (var reservation in waiting)
                {
                    reservation.reservation_status = "Avbrutt";
                    reservation.ResourceMatch.match_status = "Avbrutt";
                }

                var interests = await context.VolunteerInterests.Where(interest => interest.resource_need_id == id && (interest.interest_status == "Venter på svar" || interest.interest_status == "Godtatt")).ToListAsync();
                foreach (var interest in interests)
                {
                    interest.interest_status = "Behov avsluttet";
                }
            }

            need.status = status;
            return null;
        });
    }

    public Task<string?> AddPeriodAsync(int id, int accountId, bool volunteer, DateTime start, DateTime end)
    {
        return ChangeAsync(async () =>
        {
            if (end <= start || start <= DateTime.Now)
            {
                return "Velg en fremtidig periode med slutt etter start.";
            }

            var owned = volunteer ? await context.VolunteerProfiles.AnyAsync(profile => profile.volunteer_profile_id == id && profile.account_id == accountId && !profile.archived) : await context.Resources.AnyAsync(resource => resource.resource_id == id && resource.account_id == accountId && !resource.archived);
            if (!owned)
            {
                return "Ingen tilgang til bidraget.";
            }

            if (await context.AvailabilityPeriods.AnyAsync(period => (volunteer ? period.volunteer_profile_id == id : period.resource_id == id) && period.available_from < end && period.available_to > start))
            {
                return "Perioden overlapper en eksisterende periode.";
            }

            context.AvailabilityPeriods.Add(new Availability { resource_id = volunteer ? null : id, volunteer_profile_id = volunteer ? id : null, available_from = start, available_to = end });
            return null;
        });
    }

    public Task<string?> RemovePeriodAsync(int id, int accountId)
    {
        return ChangeAsync(async () =>
        {
            var period = await context.AvailabilityPeriods.Include(period => period.Resource).Include(period => period.VolunteerProfile).SingleOrDefaultAsync(period => period.availability_id == id);
            if (period == null || (period.Resource?.account_id ?? period.VolunteerProfile?.account_id) != accountId)
            {
                return "Ingen tilgang til perioden.";
            }

            if (period.resource_id.HasValue && await context.Reservations.AnyAsync(reservation => reservation.ResourceMatch.resource_id == period.resource_id && (reservation.reservation_status == "Godtatt" || reservation.reservation_status == "Venter på svar") && reservation.requested_start < period.available_to && reservation.requested_end > period.available_from))
            {
                return "Perioden har en aktiv forespørsel eller reservasjon.";
            }

            context.AvailabilityPeriods.Remove(period);
            return null;
        });
    }

    public Task<string?> ArchiveResourceAsync(int id, int accountId, bool restore)
    {
        return ChangeAsync(async () =>
        {
            var resource = await context.Resources.SingleOrDefaultAsync(resource => resource.resource_id == id && resource.account_id == accountId);
            if (resource == null)
            {
                return "Ressursen finnes ikke.";
            }

            if (!restore && await context.Reservations.AnyAsync(reservation => reservation.ResourceMatch.resource_id == id && (reservation.reservation_status == "Godtatt" || reservation.reservation_status == "Venter på svar")))
            {
                return "Avklar aktive forespørsler før arkivering.";
            }

            resource.archived = !restore;
            return null;
        });
    }

    public Task<string?> InterestAsync(int needId, int accountId, string message)
    {
        return ChangeAsync(async () =>
        {
            var profile = await context.VolunteerProfiles.SingleOrDefaultAsync(profile => profile.account_id == accountId && !profile.archived);
            if (profile == null)
            {
                return "Opprett en aktiv frivilligprofil før du melder interesse.";
            }

            var need = await context.ResourceNeeds.SingleOrDefaultAsync(need => need.resource_need_id == needId && !need.archived && need.status != "Løst");
            if (need == null || need.required_end <= DateTime.Now)
            {
                return "Behovet er avsluttet.";
            }

            if (need.deadline.HasValue && need.deadline < DateTime.Now)
            {
                return "Svarfristen for behovet er utløpt.";
            }

            if (await context.VolunteerInterests.AnyAsync(interest => interest.resource_need_id == needId && interest.volunteer_profile_id == profile.volunteer_profile_id && (interest.interest_status == "Venter på svar" || interest.interest_status == "Godtatt")))
            {
                return "Du har allerede meldt interesse.";
            }

            context.VolunteerInterests.Add(new VolunteerInterest { volunteer_profile_id = profile.volunteer_profile_id, resource_need_id = needId, message = message });
            return null;
        });
    }

    public Task<string?> InterestStatusAsync(int id, int accountId, bool supplier, string status)
    {
        return ChangeAsync(async () =>
        {
            var interest = await context.VolunteerInterests.Include(interest => interest.VolunteerProfile).Include(interest => interest.ResourceNeed).SingleOrDefaultAsync(interest => interest.interest_id == id);
            if (interest == null)
            {
                return "Interessen finnes ikke.";
            }

            if (supplier)
            {
                if (interest.VolunteerProfile.account_id != accountId || status != "Trukket" || interest.interest_status is not ("Venter på svar" or "Godtatt"))
                {
                    return "Ingen gyldig endring.";
                }
            }
            else if (interest.ResourceNeed.public_actor_account_id != accountId || interest.interest_status != "Venter på svar" || interest.ResourceNeed.archived || interest.ResourceNeed.status == "Løst" || status is not ("Godtatt" or "Avslått"))
            {
                return "Ingen gyldig endring.";
            }

            if (!supplier && status == "Godtatt" && interest.ResourceNeed.required_end <= DateTime.Now)
            {
                return "Behovets tidsrom er utløpt.";
            }

            interest.interest_status = status;
            return null;
        });
    }
}
