namespace DeliveryAssignmentEngine.Application.Commands.AssignDeliveryAgent;

public class DeliveryAssignmentDto
{
    public bool IsSuccessful { get; set; }
    public string Message { get; set; }
    public Guid DeliveryId { get; set; }
    public Guid AgentId { get; set; }
}
