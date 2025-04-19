using DeliveryAssignmentEngine.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryAssignmentEngine.Domain.Services;

public interface IDeliveryAssignmentService
{
    Task<AssignmentResult> AssignDeliveryToAgentAsync(DeliveryId deliveryId, DeliveryAgentId agentId);
    Task<IEnumerable<DeliveryAgentId>> FindSuitableAgentsForDeliveryAsync(DeliveryId deliveryId);
}

public class DeliveryAssignmentService : IDeliveryAssignmentService
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IDeliveryAgentRepository _deliveryAgentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeliveryAssignmentService(
        IDeliveryRepository deliveryRepository,
        IDeliveryAgentRepository deliveryAgentRepository,
        IUnitOfWork unitOfWork)
    {
        _deliveryRepository = deliveryRepository;
        _deliveryAgentRepository = deliveryAgentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AssignmentResult> AssignDeliveryToAgentAsync(DeliveryId deliveryId, DeliveryAgentId agentId)
    {
        var delivery = await _deliveryRepository.GetByIdAsync(deliveryId);
        if (delivery == null)
            return AssignmentResult.Failed("Delivery not found");

        var agent = await _deliveryAgentRepository.GetByIdAsync(agentId);
        if (agent == null)
            return AssignmentResult.Failed("Delivery agent not found");

        if (!agent.IsAvailable())
            return AssignmentResult.Failed("Delivery agent is not available");

        if (!agent.CanHandleDelivery(delivery))
            return AssignmentResult.Failed("Delivery agent cannot handle this delivery");

        delivery.AssignToAgent(agentId);
        agent.AssignDelivery(deliveryId);

        await _unitOfWork.SaveChangesAsync();

        return AssignmentResult.Successful();
    }

    public async Task<IEnumerable<DeliveryAgentId>> FindSuitableAgentsForDeliveryAsync(DeliveryId deliveryId)
    {
        var delivery = await _deliveryRepository.GetByIdAsync(deliveryId);
        if (delivery == null)
            return Enumerable.Empty<DeliveryAgentId>();

        var availableAgents = await _deliveryAgentRepository.GetAvailableAgentsAsync();

        return availableAgents
            .Where(agent => agent.CanHandleDelivery(delivery))
            .Select(agent => agent.Id)
            .ToList();
    }
}

public class AssignmentResult
{
    public bool IsSuccessful { get; }
    public string ErrorMessage { get; }

    private AssignmentResult(bool isSuccessful, string errorMessage)
    {
        IsSuccessful = isSuccessful;
        ErrorMessage = errorMessage;
    }

    public static AssignmentResult Successful() => new(true, string.Empty);
    public static AssignmentResult Failed(string errorMessage) => new(false, errorMessage);
}