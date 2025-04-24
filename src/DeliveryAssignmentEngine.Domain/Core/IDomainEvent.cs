namespace DeliveryAssignmentEngine.Domain.Core;

/// <summary>
/// Interface for domain events
/// </summary>
public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
}
