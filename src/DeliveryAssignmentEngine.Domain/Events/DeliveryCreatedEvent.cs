using DeliveryAssignmentEngine.Domain.Core;
using DeliveryAssignmentEngine.Domain.ValueObjects;

namespace DeliveryAssignmentEngine.Domain.Events;

// Delivery Events
public class DeliveryCreatedEvent(
    DeliveryId deliveryId, 
    CustomerId customerId) : DomainEvent
{
    public DeliveryId DeliveryId { get; } = deliveryId;
    public CustomerId CustomerId { get; } = customerId;
}
