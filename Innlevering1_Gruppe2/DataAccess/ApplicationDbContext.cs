using Microsoft.EntityFrameworkCore;
using Reachout;
using Reachout.Models.Entities;

namespace Reachout.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Resource> Resources { get; set; }
        public DbSet<ProviderAccount> ProviderAccounts { get; set; }
        public DbSet<ResourceType> ResourceTypes { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<PublicOrganization> PublicOrganizations { get; set; }
        public DbSet<PublicActorAccount> PublicActorAccounts { get; set; }
        public DbSet<ResourceNeed> ResourceNeeds { get; set; }
        public DbSet<Availability> AvailabilityPeriods { get; set; }
        public DbSet<VolunteerProfile> VolunteerProfiles { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<VolunteerProfileSkill> VolunteerProfileSkills { get; set; }
        public DbSet<VolunteerInterest> VolunteerInterests { get; set; }
        public DbSet<ResourceMatch> ResourceMatches { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<OrganizationInvitation> OrganizationInvitations { get; set; }
        public DbSet<AccountToken> AccountTokens { get; set; }
        public DbSet<AuditEntry> AuditEntries { get; set; }
        public DbSet<AppWriteLock> AppWriteLocks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Resource>(entity =>
            {
                entity.ToTable("Resource");
                // Primærnøkkel
                entity.HasKey(e => e.resource_id);
                // Vanlige kolonner
                entity.Property(e => e.resource_name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.description).HasMaxLength(4000).IsRequired();
                entity.Property(e => e.contact_details).HasMaxLength(1000).IsRequired();
                entity.Property(e => e.verification_status).HasMaxLength(75).IsRequired();
                // Fremmednøkkel til kontoen som eier ressursen
                entity.HasOne(e => e.ProviderAccount).WithMany(provider => provider.Resources).HasForeignKey(e => e.account_id).OnDelete(DeleteBehavior.Restrict);
                // Fremmednøkkel til ResourceType
                entity.HasOne(e => e.ResourceType).WithMany().HasForeignKey(e => e.resource_type_id).OnDelete(DeleteBehavior.Restrict);
                // Fremmednøkkel til Location
                entity.HasOne(e => e.Location).WithMany(l => l.Resources).HasForeignKey(e => e.location_id).OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");
                entity.HasKey(e => e.account_id);
                entity.Property(e => e.account_type).HasMaxLength(50).IsRequired();
                entity.Property(e => e.email).HasMaxLength(255).IsRequired();
                entity.Property(e => e.contact_name).HasMaxLength(200).HasDefaultValue(string.Empty).IsRequired();
                entity.Property(e => e.phone).HasMaxLength(50).HasDefaultValue(string.Empty).IsRequired();
                entity.HasIndex(e => e.email).IsUnique();
                entity.Property(e => e.password_hash).HasMaxLength(255).IsRequired();
            });
            modelBuilder.Entity<ProviderAccount>(entity =>
            {
                entity.ToTable("ProviderAccount");
                entity.HasKey(e => e.account_id);
                entity.Property(e => e.provider_type).HasMaxLength(100).IsRequired();
                entity.Property(e => e.contact_name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.phone).HasMaxLength(50);
                entity.Property(e => e.address).HasMaxLength(100);
                entity.Property(e => e.organization_name).HasMaxLength(200);
                entity.Property(e => e.organization_number).HasMaxLength(100);
                entity.HasOne(provider => provider.Account).WithOne().HasForeignKey<ProviderAccount>(e => e.account_id).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<ResourceType>(entity =>
            {
                entity.ToTable("ResourceType");
                entity.HasKey(e => e.resource_type_id);
                entity.Property(e => e.category_name).HasMaxLength(200).IsRequired();
                entity.HasIndex(e => e.category_name).IsUnique();
                entity.HasData(new ResourceType { resource_type_id = 1, category_name = "Kjøretøy" }, new ResourceType { resource_type_id = 2, category_name = "Heisekran" }, new ResourceType { resource_type_id = 3, category_name = "Frivillige" }, new ResourceType { resource_type_id = 4, category_name = "Tilhenger" });
            });
            modelBuilder.Entity<PublicOrganization>().HasKey(entity => entity.public_organization_id);
            modelBuilder.Entity<PublicActorAccount>().HasKey(entity => entity.account_id);
            modelBuilder.Entity<PublicActorAccount>().HasOne(entity => entity.Account).WithOne().HasForeignKey<PublicActorAccount>(entity => entity.account_id).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PublicActorAccount>().HasOne(entity => entity.PublicOrganization).WithMany().HasForeignKey(entity => entity.public_organization_id).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ResourceNeed>().HasKey(entity => entity.resource_need_id);
            modelBuilder.Entity<ResourceNeed>().HasOne(entity => entity.PublicActorAccount).WithMany().HasForeignKey(entity => entity.public_actor_account_id).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ResourceNeed>().HasOne(entity => entity.ResourceType).WithMany().HasForeignKey(entity => entity.resource_type_id).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ResourceNeed>().HasOne(entity => entity.Location).WithMany().HasForeignKey(entity => entity.location_id).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<VolunteerProfile>().HasKey(entity => entity.volunteer_profile_id);
            modelBuilder.Entity<VolunteerProfile>().HasIndex(entity => entity.account_id).IsUnique();
            modelBuilder.Entity<VolunteerProfile>().HasOne(entity => entity.ProviderAccount).WithOne().HasForeignKey<VolunteerProfile>(entity => entity.account_id).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Skill>().HasKey(entity => entity.skill_id);
            modelBuilder.Entity<Skill>().HasIndex(entity => entity.name).IsUnique();
            modelBuilder.Entity<VolunteerProfileSkill>().HasKey(entity => new { entity.volunteer_profile_id, entity.skill_id });
            modelBuilder.Entity<VolunteerProfileSkill>().HasOne(entity => entity.VolunteerProfile).WithMany(profile => profile.VolunteerProfileSkills).HasForeignKey(entity => entity.volunteer_profile_id).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<VolunteerProfileSkill>().HasOne(entity => entity.Skill).WithMany().HasForeignKey(entity => entity.skill_id).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Availability>().HasKey(entity => entity.availability_id);
            modelBuilder.Entity<Availability>().HasOne(entity => entity.Resource).WithMany().HasForeignKey(entity => entity.resource_id).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Availability>().HasOne(entity => entity.VolunteerProfile).WithMany().HasForeignKey(entity => entity.volunteer_profile_id).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Availability>().ToTable("Availability", table =>
            {
                table.HasCheckConstraint("CK_Availability_Owner", "(resource_id IS NOT NULL) <> (volunteer_profile_id IS NOT NULL)");
                table.HasCheckConstraint("CK_Availability_Period", "available_to > available_from");
            });
            modelBuilder.Entity<VolunteerInterest>().HasKey(entity => entity.interest_id);
            modelBuilder.Entity<VolunteerInterest>().HasOne(entity => entity.VolunteerProfile).WithMany().HasForeignKey(entity => entity.volunteer_profile_id).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<VolunteerInterest>().HasOne(entity => entity.ResourceNeed).WithMany().HasForeignKey(entity => entity.resource_need_id).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ResourceMatch>().HasKey(entity => entity.resource_match_id);
            modelBuilder.Entity<ResourceMatch>().HasIndex(entity => entity.resource_need_id).IsUnique();
            modelBuilder.Entity<ResourceMatch>().HasOne(entity => entity.Resource).WithMany().HasForeignKey(entity => entity.resource_id).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ResourceMatch>().HasOne(entity => entity.ResourceNeed).WithMany().HasForeignKey(entity => new { entity.resource_need_id, entity.public_actor_account_id }).HasPrincipalKey(need => new { need.resource_need_id, need.public_actor_account_id }).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ResourceMatch>().HasOne(entity => entity.PublicActorAccount).WithMany().HasForeignKey(entity => entity.public_actor_account_id).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Reservation>().HasKey(entity => entity.reservation_id);
            modelBuilder.Entity<Reservation>().HasIndex(entity => entity.resource_match_id).IsUnique();
            modelBuilder.Entity<Reservation>().HasOne(entity => entity.ResourceMatch).WithOne().HasForeignKey<Reservation>(entity => new { entity.resource_match_id, entity.public_actor_account_id }).HasPrincipalKey<ResourceMatch>(match => new { match.resource_match_id, match.public_actor_account_id }).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OrganizationInvitation>().HasKey(entity => entity.invitation_id);
            modelBuilder.Entity<OrganizationInvitation>().HasIndex(entity => entity.token_hash).IsUnique();
            modelBuilder.Entity<OrganizationInvitation>().HasOne(entity => entity.PublicOrganization).WithMany().HasForeignKey(entity => entity.public_organization_id).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AccountToken>().HasKey(entity => entity.account_token_id);
            modelBuilder.Entity<AccountToken>().HasIndex(entity => entity.token_hash).IsUnique();
            modelBuilder.Entity<AccountToken>().HasOne(entity => entity.Account).WithMany().HasForeignKey(entity => entity.account_id).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AuditEntry>().HasKey(entity => entity.audit_entry_id);
            modelBuilder.Entity<AppWriteLock>().HasKey(entity => entity.Id);
            modelBuilder.Entity<AppWriteLock>().HasData(new AppWriteLock { Id = 1 });
            modelBuilder.Entity<Resource>().HasIndex(entity => entity.location_id).IsUnique();
            modelBuilder.Entity<Resource>().Property(entity => entity.image_data).HasColumnType("mediumblob");
            modelBuilder.Entity<Account>().Property(entity => entity.security_stamp).HasMaxLength(100);
            modelBuilder.Entity<Resource>().Property(entity => entity.availability_status).HasMaxLength(40);
            modelBuilder.Entity<Resource>().Property(entity => entity.image_content_type).HasMaxLength(30);
            modelBuilder.Entity<ResourceType>().HasData(new ResourceType { resource_type_id = 5, category_name = "Utstyr" }, new ResourceType { resource_type_id = 6, category_name = "Lokaler" }, new ResourceType { resource_type_id = 7, category_name = "Strøm" }, new ResourceType { resource_type_id = 8, category_name = "Vann" });
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                if (entity.ClrType.Namespace == typeof(Account).Namespace && entity.ClrType != typeof(ResourceType))
                {
                    entity.SetTableName(entity.ClrType.Name);
                }
            }

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("Location");
                entity.HasKey(e => e.location_id);
                entity.Property(e => e.address_or_area).HasMaxLength(500).IsRequired();
                entity.Property(e => e.latitude).HasPrecision(9, 6).IsRequired();
                entity.Property(e => e.longitude).HasPrecision(10, 7).IsRequired();
            });
        }
    }
}
