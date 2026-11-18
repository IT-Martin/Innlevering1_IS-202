using Reachout.Models.Entities;

namespace Reachout.DataAccess.Repositories;
public interface IResourceRepository
{
    Task<IReadOnlyList<Resource>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Resource>> GetByAccountIdAsync(int accountId, CancellationToken cancellationToken = default);
    Task<Resource?> GetByIdForAccountAsync(int id, int accountId, CancellationToken cancellationToken = default);
    Task<Resource> AddAsync(Resource resource, CancellationToken cancellationToken = default);
    Task UpdateAsync(Resource resource, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, int accountId, CancellationToken cancellationToken = default);
}
