using MediatR;

namespace DeliveryAssignmentEngine.Application.Queries.FindSuitableAgents;

public class FindSuitableAgentsQuery : IRequest<IEnumerable<Guid>>
{
    public Guid DeliveryId { get; set; }
}
