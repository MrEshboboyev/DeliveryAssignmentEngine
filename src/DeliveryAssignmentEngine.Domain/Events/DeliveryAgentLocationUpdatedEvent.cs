using DeliveryAssignmentEngine.Domain.Core;
using DeliveryAssignmentEngine.Domain.ValueObjects;

namespace DeliveryAssignmentEngine.Domain.Events;

public class DeliveryAgentLocationUpdatedEvent(
    DeliveryAgentId agentId, 
    Location newLocation) : DomainEvent
{
    public DeliveryAgentId AgentId { get; } = agentId;
    public Location NewLocation { get; } = newLocation;
}
