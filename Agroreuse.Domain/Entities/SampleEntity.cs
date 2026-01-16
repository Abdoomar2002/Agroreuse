using Agroreuse.Domain.Common;

namespace Agroreuse.Domain.Entities;

/// <summary>
/// Sample entity demonstrating DDD entity structure.
/// Replace this with your actual domain entities.
/// </summary>
public class SampleEntity : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    private SampleEntity() { } // Required for EF Core

    public static SampleEntity Create(string name, string description)
    {
        var entity = new SampleEntity
        {
            Name = name,
            Description = description,
            IsActive = true
        };

        entity.AddDomainEvent(new SampleEntityCreatedEvent(entity.Id, name));

        return entity;
    }

    public void Update(string name, string description)
    {
        Name = name;
        Description = description;
        SetUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        SetUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdated();
    }
}

public class SampleEntityCreatedEvent : DomainEvent
{
    public Guid EntityId { get; }
    public string Name { get; }

    public SampleEntityCreatedEvent(Guid entityId, string name)
    {
        EntityId = entityId;
        Name = name;
    }
}
