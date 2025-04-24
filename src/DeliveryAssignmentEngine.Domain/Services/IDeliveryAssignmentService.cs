using DeliveryAssignmentEngine.Domain.ValueObjects;

namespace DeliveryAssignmentEngine.Domain.Services;

public interface IDeliveryAssignmentService
{
    Task<AssignmentResult> AssignDeliveryToAgentAsync(DeliveryId deliveryId, DeliveryAgentId agentId);
    Task<IEnumerable<DeliveryAgentId>> FindSuitableAgentsForDeliveryAsync(DeliveryId deliveryId);
}
