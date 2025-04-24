namespace DeliveryAssignmentEngine.Domain.Core;

/// <summary>
/// Marker interface for aggregate roots
/// </summary>
public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}
