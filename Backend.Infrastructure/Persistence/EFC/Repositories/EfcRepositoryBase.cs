using Backend.Application.Abstractions.Persistence;

namespace Backend.Infrastructure.Persistence.EFC.Repositories;

public abstract class EfcRepositoryBase<TEntity, TKey, TModel> : IRepositoryBase<TModel, TKey> where TEntity : class, IEntity<TKey>
{
    public abstract Task AddAsync(TModel model, CancellationToken ct = default);

    public abstract Task UpdateAsync(TModel model, CancellationToken ct = default);

    public Task DeleteAsync(TModel model, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteByIdAsync(TKey id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<TModel?> GetByIdAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<TModel?>> ListAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }


}
