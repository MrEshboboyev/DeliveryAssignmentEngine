using MediatR;

namespace DeliveryAssignmentEngine.Application.Commands.AssignDeliveryAgent;

public class AssignDeliveryAgentCommand : IRequest<AssignmentResultDto>
{
    public Guid DeliveryId { get; set; }
    public Guid AgentId { get; set; }
}
