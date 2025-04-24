using DeliveryAssignmentEngine.Domain.Core;
using DeliveryAssignmentEngine.Domain.ValueObjects;

namespace DeliveryAssignmentEngine.Domain.Events;

public class DeliveryAssignedEvent(
    DeliveryId deliveryId, 
    DeliveryAgentId agentId) : DomainEvent
{
    public DeliveryId DeliveryId { get; } = deliveryId;
    public DeliveryAgentId AgentId { get; } = agentId;
}
