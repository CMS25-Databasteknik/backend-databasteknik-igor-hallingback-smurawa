using Backend.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Persistence.EFC.Repositories;

public abstract class EfcRepositoryBase<TEntity, TKey, TModel>(DbContext context) : IRepositoryBase<TModel, TKey> where TEntity : class, IEntity<TKey>
{
    protected DbContext Context { get; } = context;
    protected DbSet<TEntity> Set => Context.Set<TEntity>();
    public abstract TModel ToModel(TEntity entity);

    public abstract Task AddAsync(TModel model, CancellationToken ct = default);

    public abstract Task UpdateAsync(TModel model, CancellationToken ct = default);

    public virtual async Task DeleteAsync(TKey id, CancellationToken ct = default)
    {
        var entity = await Set.SingleOrDefaultAsync(e => e.Id!.Equals(id), ct);
        if (entity is null) return;
        {
            Set.Remove(entity);
        }
    }

    public virtual async Task DeleteByIdAsync(TKey id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public virtual async Task<TModel?> GetByIdAsync(TKey id, CancellationToken ct = default)
    {
        var entity = await Set.AsNoTracking().SingleOrDefaultAsync(e => e.Id!.Equals(id), ct);
        return entity is null ? default : ToModel(entity);
    }

    public virtual async Task<IReadOnlyList<TModel?>> ListAsync(CancellationToken ct = default)
    {
        var entities = await Set.AsNoTracking().ToListAsync(ct);
        return [.. entities.Select(ToModel)];
    }
}
