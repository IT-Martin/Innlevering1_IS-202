using Innlevering1_Gruppe2.Models.Entities;

namespace Innlevering1_Gruppe2.DataAccess.Repositories;

public interface IResourceRepository
{
    Task<IReadOnlyList<Resource>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Resource>> GetByAccountIdAsync(int accountId, CancellationToken cancellationToken = default);
    Task<Resource?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Resource?> GetByIdForAccountAsync(int id, int accountId, CancellationToken cancellationToken = default);
    Task<Resource> AddAsync(Resource resource, CancellationToken cancellationToken = default);
    Task UpdateAsync(Resource resource, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, int accountId, CancellationToken cancellationToken = default);
}
