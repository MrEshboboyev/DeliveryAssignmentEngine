using DeliveryAssignmentEngine.Domain.Core;
using DeliveryAssignmentEngine.Domain.ValueObjects;

namespace DeliveryAssignmentEngine.Domain.Events;

public class DeliveryAgentRatingUpdatedEvent(
    DeliveryAgentId agentId, 
    DeliveryAgentRating newRating) : DomainEvent
{
    public DeliveryAgentId AgentId { get; } = agentId;
    public DeliveryAgentRating NewRating { get; } = newRating;
}
