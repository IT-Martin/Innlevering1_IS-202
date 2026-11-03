using Innlevering1_Gruppe2.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Innlevering1_Gruppe2.DataAccess.Repositories;

public class ResourceRepository(ApplicationDbContext context) : IResourceRepository
{
    public async Task<IReadOnlyList<Resource>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await context.Resources
            .Include(resource => resource.Location)
            .Include(resource => resource.Account)
            .Include(resource => resource.ResourceType)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<Resource?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return context.Resources
            .Include(resource => resource.Location)
            .Include(resource => resource.Account)
            .Include(resource => resource.ResourceType)
            .AsNoTracking()
            .FirstOrDefaultAsync(
                resource => resource.resource_id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Resource>> GetByAccountIdAsync(
        int accountId,
        CancellationToken cancellationToken = default)
    {
        return await context.Resources
            .Include(resource => resource.Location)
            .Include(resource => resource.ResourceType)
            .Where(resource => resource.account_id == accountId)
            .AsNoTracking()
            .OrderByDescending(resource => resource.resource_id)
            .ToListAsync(cancellationToken);
    }

    public Task<Resource?> GetByIdForAccountAsync(
        int id,
        int accountId,
        CancellationToken cancellationToken = default)
    {
        return context.Resources
            .Include(resource => resource.Location)
            .Include(resource => resource.ResourceType)
            .FirstOrDefaultAsync(
                resource => resource.resource_id == id
                    && resource.account_id == accountId,
                cancellationToken);
    }

    public async Task<Resource> AddAsync(
        Resource resource,
        CancellationToken cancellationToken = default)
    {
        await context.Resources.AddAsync(resource, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return resource;
    }

    public async Task UpdateAsync(
        Resource resource,
        CancellationToken cancellationToken = default)
    {
        context.Resources.Update(resource);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(
        int id,
        int accountId,
        CancellationToken cancellationToken = default)
    {
        var resource = await context.Resources
            .FirstOrDefaultAsync(
                resource => resource.resource_id == id
                    && resource.account_id == accountId,
                cancellationToken);

        if (resource is null)
        {
            return false;
        }

        context.Resources.Remove(resource);
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}