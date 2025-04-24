using DeliveryAssignmentEngine.Domain.Services;
using DeliveryAssignmentEngine.Domain.ValueObjects;
using MediatR;

namespace DeliveryAssignmentEngine.Application.Queries.FindSuitableAgents;

public class FindSuitableAgentsQueryHandler(IDeliveryAssignmentService deliveryAssignmentService) : IRequestHandler<FindSuitableAgentsQuery, IEnumerable<Guid>>
{
    public async Task<IEnumerable<Guid>> Handle(
        FindSuitableAgentsQuery request, 
        CancellationToken cancellationToken)
    {
        var deliveryId = new DeliveryId(request.DeliveryId);

        var suitableAgentIds = await deliveryAssignmentService.FindSuitableAgentsForDeliveryAsync(deliveryId);

        return suitableAgentIds.Select(id => id.Value);
    }
}
