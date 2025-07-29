namespace TaskManagement.Domain;

public abstract class BaseEntity<TId> : IEntity<TId> where TId : IComparable<TId>
{
    public TId Id { get; init; } = default!;
    public int SequentialId { get; init; }
}
