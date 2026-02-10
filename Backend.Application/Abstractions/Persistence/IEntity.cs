namespace Backend.Application.Abstractions.Persistence;

public interface IEntity<TKey>
{
    TKey Id { get; }
}