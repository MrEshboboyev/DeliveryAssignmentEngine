namespace DeliveryAssignmentEngine.Domain.Core;

/// <summary>
/// Aggregate Root is a special type of entity that ensures the consistency of changes
/// made within the aggregate by defining its boundaries.
/// </summary>
public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot where TId : notnull
{
    /// <summary>
    /// Version number for optimistic concurrency control
    /// </summary>
    public int Version { get; protected set; }

    protected AggregateRoot() { }

    protected AggregateRoot(TId id) : base(id) { }

    /// <summary>
    /// Applies a versioned event to update the aggregate state and 
    /// records the event for eventual consistency
    /// </summary>
    protected void ApplyDomainEvent<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent
    {
        // Apply changes to the aggregate
        ((dynamic)this).Apply((dynamic)domainEvent);

        // Record the event
        AddDomainEvent(domainEvent);

        // Increment the version
        Version++;
    }
}
