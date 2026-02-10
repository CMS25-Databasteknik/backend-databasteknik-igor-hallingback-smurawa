namespace Backend.Application.Abstractions.Persistence;

public interface IRepositoryBase<TModel, in TKey>
{
    Task<TModel?> GetByIdAsync(CancellationToken ct = default);
    Task<IReadOnlyList<TModel?>> ListAsync(CancellationToken ct = default);
    Task AddAsync(TModel model, CancellationToken ct = default);
    Task UpdateAsync(TModel model, CancellationToken ct = default);
    Task DeleteAsync(TModel model, CancellationToken ct = default);
    Task DeleteByIdAsync(TKey id, CancellationToken ct = default);
}
