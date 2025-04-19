using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryAssignmentEngine.Application.Commands;

public class AssignDeliveryAgentCommand
{
    public Guid DeliveryId { get; set; }
    public Guid AgentId { get; set; }
}

public class FindSuitableAgentsQuery
{
    public Guid DeliveryId { get; set; }
}

public class AssignDeliveryAgentCommandHandler : IRequestHandler<AssignDeliveryAgentCommand, AssignmentResultDto>
{
    private readonly IDeliveryAssignmentService _deliveryAssignmentService;

    public AssignDeliveryAgentCommandHandler(IDeliveryAssignmentService deliveryAssignmentService)
    {
        _deliveryAssignmentService = deliveryAssignmentService;
    }

    public async Task<AssignmentResultDto> Handle(AssignDeliveryAgentCommand request, CancellationToken cancellationToken)
    {
        var deliveryId = new DeliveryId(request.DeliveryId);
        var agentId = new DeliveryAgentId(request.AgentId);

        var result = await _deliveryAssignmentService.AssignDeliveryToAgentAsync(deliveryId, agentId);

        return new AssignmentResultDto
        {
            IsSuccessful = result.IsSuccessful,
            ErrorMessage = result.ErrorMessage
        };
    }
}

public class FindSuitableAgentsQueryHandler : IRequestHandler<FindSuitableAgentsQuery, IEnumerable<Guid>>
{
    private readonly IDeliveryAssignmentService _deliveryAssignmentService;

    public FindSuitableAgentsQueryHandler(IDeliveryAssignmentService deliveryAssignmentService)
    {
        _deliveryAssignmentService = deliveryAssignmentService;
    }

    public async Task<IEnumerable<Guid>> Handle(FindSuitableAgentsQuery request, CancellationToken cancellationToken)
    {
        var deliveryId = new DeliveryId(request.DeliveryId);

        var suitableAgentIds = await _deliveryAssignmentService.FindSuitableAgentsForDeliveryAsync(deliveryId);

        return suitableAgentIds.Select(id => id.Value);
    }
}

public class DeliveryAssignmentApplicationService
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IDeliveryAgentRepository _deliveryAgentRepository;
    private readonly IDeliveryAssignmentService _deliveryAssignmentService;
    private readonly IUnitOfWork _unitOfWork;

    public DeliveryAssignmentApplicationService(
        IDeliveryRepository deliveryRepository,
        IDeliveryAgentRepository deliveryAgentRepository,
        IDeliveryAssignmentService deliveryAssignmentService,
        IUnitOfWork unitOfWork)
    {
        _deliveryRepository = deliveryRepository;
        _deliveryAgentRepository = deliveryAgentRepository;
        _deliveryAssignmentService = deliveryAssignmentService;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeliveryAssignmentDto> AssignBestAgentToDeliveryAsync(Guid deliveryId)
    {
        var delivery = await _deliveryRepository.GetByIdAsync(new DeliveryId(deliveryId));
        if (delivery == null)
            throw new DeliveryNotFoundException(deliveryId);

        var suitableAgentIds = await _deliveryAssignmentService.FindSuitableAgentsForDeliveryAsync(delivery.Id);
        if (!suitableAgentIds.Any())
            return new DeliveryAssignmentDto { IsSuccessful = false, Message = "No suitable agents found" };

        // Find the best agent using sophisticated algorithm
        var bestAgentId = await FindBestAgentForDeliveryAsync(delivery, suitableAgentIds);
        if (bestAgentId == null)
            return new DeliveryAssignmentDto { IsSuccessful = false, Message = "Could not determine best agent" };

        var result = await _deliveryAssignmentService.AssignDeliveryToAgentAsync(delivery.Id, bestAgentId);

        return new DeliveryAssignmentDto
        {
            IsSuccessful = result.IsSuccessful,
            Message = result.IsSuccessful ? "Assignment successful" : result.ErrorMessage,
            DeliveryId = deliveryId,
            AgentId = bestAgentId.Value
        };
    }

    private async Task<DeliveryAgentId?> FindBestAgentForDeliveryAsync(
        Delivery delivery,
        IEnumerable<DeliveryAgentId> suitableAgentIds)
    {
        var suitableAgents = new List<DeliveryAgent>();

        foreach (var agentId in suitableAgentIds)
        {
            var agent = await _deliveryAgentRepository.GetByIdAsync(agentId);
            if (agent != null)
                suitableAgents.Add(agent);
        }

        if (!suitableAgents.Any())
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

public class DeliveryAssignmentDto
{
    public bool IsSuccessful { get; set; }
    public string Message { get; set; }
    public Guid DeliveryId { get; set; }
    public Guid AgentId { get; set; }
}

public class AssignmentResultDto
{
    public bool IsSuccessful { get; set; }
    public string ErrorMessage { get; set; }
}