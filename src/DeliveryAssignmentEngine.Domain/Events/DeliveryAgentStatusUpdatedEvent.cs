using DeliveryAssignmentEngine.Domain.Core;
using DeliveryAssignmentEngine.Domain.Enums;
using DeliveryAssignmentEngine.Domain.ValueObjects;

namespace DeliveryAssignmentEngine.Domain.Events;

public class DeliveryAgentStatusUpdatedEvent(
    DeliveryAgentId agentId, 
    DeliveryAgentStatus newStatus) : DomainEvent
{
    public DeliveryAgentId AgentId { get; } = agentId;
    public DeliveryAgentStatus NewStatus { get; } = newStatus;
}
