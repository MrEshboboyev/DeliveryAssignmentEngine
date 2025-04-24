using DeliveryAssignmentEngine.Application.Commands.AssignDeliveryAgent;
using DeliveryAssignmentEngine.Domain.Entities;
using DeliveryAssignmentEngine.Domain.Repositories;
using DeliveryAssignmentEngine.Domain.Services;
using DeliveryAssignmentEngine.Domain.ValueObjects;

namespace DeliveryAssignmentEngine.Application.Services;

public class DeliveryAssignmentApplicationService(
    IDeliveryRepository deliveryRepository,
    IDeliveryAgentRepository deliveryAgentRepository,
    IDeliveryAssignmentService deliveryAssignmentService)
{
    public async Task<DeliveryAssignmentDto> AssignBestAgentToDeliveryAsync(Guid deliveryId)
    {
        var delivery = await deliveryRepository.GetByIdAsync(new DeliveryId(deliveryId))
            ?? throw new DeliveryNotFoundException(deliveryId);

        var suitableAgentIds = await deliveryAssignmentService.FindSuitableAgentsForDeliveryAsync(delivery.Id);
        if (!suitableAgentIds.Any())
            return new DeliveryAssignmentDto { IsSuccessful = false, Message = "No suitable agents found" };

        // Find the best agent using sophisticated algorithm
        var bestAgentId = await FindBestAgentForDeliveryAsync(delivery, suitableAgentIds);
        if (bestAgentId == null)
            return new DeliveryAssignmentDto { IsSuccessful = false, Message = "Could not determine best agent" };

        var result = await deliveryAssignmentService.AssignDeliveryToAgentAsync(delivery.Id, bestAgentId);

        return new DeliveryAssignmentDto
        {
            IsSuccessful = result.IsSuccessful,
            Message = result.IsSuccessful ? "Assignment successful" : result.ErrorMessage,
            DeliveryId = deliveryId,
            AgentId = bestAgentId.Value
        };
    }

    private async Task<DeliveryAgentId> FindBestAgentForDeliveryAsync(
        Delivery delivery,
        IEnumerable<DeliveryAgentId> suitableAgentIds)
    {
        var suitableAgents = new List<DeliveryAgent>();

        foreach (var agentId in suitableAgentIds)
        {
            var agent = await deliveryAgentRepository.GetByIdAsync(agentId);
            if (agent != null)
                suitableAgents.Add(agent);
        }

        if (suitableAgents.Count == 0)
            return null;

        // Advanced algorithm to find the best agent
        // Consider factors like:
        // 1. Distance to pickup location
        // 2. Current workload
        // 3. Historical performance
        // 4. Specialization match

        return suitableAgents
            .OrderBy(a => a.CurrentLocation.DistanceTo(delivery.PickupLocation))
            .ThenBy(a => a.AssignedDeliveries.Count)
            .FirstOrDefault()?.Id;
    }
}
