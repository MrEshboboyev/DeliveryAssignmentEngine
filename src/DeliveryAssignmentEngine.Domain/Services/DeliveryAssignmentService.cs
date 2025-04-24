using DeliveryAssignmentEngine.Domain.Repositories;
using DeliveryAssignmentEngine.Domain.ValueObjects;

namespace DeliveryAssignmentEngine.Domain.Services;

public class DeliveryAssignmentService(
    IDeliveryRepository deliveryRepository,
    IDeliveryAgentRepository deliveryAgentRepository,
    IUnitOfWork unitOfWork) : IDeliveryAssignmentService
{
    public async Task<AssignmentResult> AssignDeliveryToAgentAsync(
        DeliveryId deliveryId,
        DeliveryAgentId agentId)
    {
        var delivery = await deliveryRepository.GetByIdAsync(deliveryId);
        if (delivery == null)
            return AssignmentResult.Failed("Delivery not found");

        var agent = await deliveryAgentRepository.GetByIdAsync(agentId);
        if (agent == null)
            return AssignmentResult.Failed("Delivery agent not found");

        if (!agent.IsAvailable())
            return AssignmentResult.Failed("Delivery agent is not available");

        if (!agent.CanHandleDelivery(delivery))
            return AssignmentResult.Failed("Delivery agent cannot handle this delivery");

        delivery.AssignToAgent(agentId);
        agent.AssignDelivery(deliveryId);

        await unitOfWork.SaveChangesAsync();

        return AssignmentResult.Successful();
    }

    public async Task<IEnumerable<DeliveryAgentId>> FindSuitableAgentsForDeliveryAsync(DeliveryId deliveryId)
    {
        var delivery = await deliveryRepository.GetByIdAsync(deliveryId);
        if (delivery == null)
            return [];

        var availableAgents = await deliveryAgentRepository.GetAvailableAgentsAsync();

        return [.. availableAgents
            .Where(agent => agent.CanHandleDelivery(delivery))
            .Select(agent => agent.Id)];
    }
}
