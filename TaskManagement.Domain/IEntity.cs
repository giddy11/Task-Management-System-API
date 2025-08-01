namespace TaskManagement.Domain;

public interface IEntity<TId> where TId : IComparable<TId>
{
    TId Id { get; init; }
    public int SequentialId { get; init; }
}
