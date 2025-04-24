using DeliveryAssignmentEngine.Domain.Core;
using DeliveryAssignmentEngine.Domain.ValueObjects;

namespace DeliveryAssignmentEngine.Domain.Events;

public class DeliveryCompletedByAgentEvent(
    DeliveryAgentId agentId, 
    DeliveryId deliveryId) : DomainEvent
{
    public DeliveryAgentId AgentId { get; } = agentId;
    public DeliveryId DeliveryId { get; } = deliveryId;
}
