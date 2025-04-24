using DeliveryAssignmentEngine.Domain.Core;
using DeliveryAssignmentEngine.Domain.ValueObjects;

namespace DeliveryAssignmentEngine.Domain.Events;

// Delivery Agent Events
public class DeliveryAgentCreatedEvent(DeliveryAgentId agentId) : DomainEvent
{
    public DeliveryAgentId AgentId { get; } = agentId;
}
