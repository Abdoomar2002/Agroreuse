namespace Agroreuse.Domain.Common;

/// <summary>
/// Base class for aggregate roots.
/// </summary>
public abstract class AggregateRoot : Entity, IAggregateRoot
{
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }

    protected AggregateRoot() : base()
    {
        CreatedAt = DateTime.UtcNow;
    }

    protected AggregateRoot(Guid id) : base(id)
    {
        CreatedAt = DateTime.UtcNow;
    }

    protected void SetUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
