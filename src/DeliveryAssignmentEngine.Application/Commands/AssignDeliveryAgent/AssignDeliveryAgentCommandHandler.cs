using DeliveryAssignmentEngine.Domain.Services;
using DeliveryAssignmentEngine.Domain.ValueObjects;
using MediatR;

namespace DeliveryAssignmentEngine.Application.Commands.AssignDeliveryAgent;

public class AssignDeliveryAgentCommandHandler(
    IDeliveryAssignmentService deliveryAssignmentService) : IRequestHandler<AssignDeliveryAgentCommand, AssignmentResultDto>
{
    public async Task<AssignmentResultDto> Handle(
        AssignDeliveryAgentCommand request, 
        CancellationToken cancellationToken)
    {
        var deliveryId = new DeliveryId(request.DeliveryId);
        var agentId = new DeliveryAgentId(request.AgentId);

        var result = await deliveryAssignmentService.AssignDeliveryToAgentAsync(deliveryId, agentId);

        return new AssignmentResultDto
        {
            IsSuccessful = result.IsSuccessful,
            ErrorMessage = result.ErrorMessage
        };
    }
}
