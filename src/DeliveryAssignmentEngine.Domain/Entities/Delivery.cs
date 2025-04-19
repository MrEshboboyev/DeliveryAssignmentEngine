namespace DeliveryAssignmentEngine.Domain.Entities;

public class Delivery : AggregateRoot<DeliveryId>
{
    public DeliveryStatus Status { get; private set; }
    public CustomerId CustomerId { get; private set; }
    public Location PickupLocation { get; private set; }
    public Location DropoffLocation { get; private set; }
    public TimeWindow DeliveryWindow { get; private set; }
    public PackageSize PackageSize { get; private set; }
    public DeliveryAgentId? AssignedAgentId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Delivery(
        DeliveryId id,
        CustomerId customerId,
        Location pickupLocation,
        Location dropoffLocation,
        TimeWindow deliveryWindow,
        PackageSize packageSize)
    {
        Id = id;
        CustomerId = customerId;
        PickupLocation = pickupLocation;
        DropoffLocation = dropoffLocation;
        DeliveryWindow = deliveryWindow;
        PackageSize = packageSize;
        Status = DeliveryStatus.Created;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new DeliveryCreatedEvent(id, customerId));
    }

    public void AssignToAgent(DeliveryAgentId agentId)
    {
        if (Status != DeliveryStatus.Created && Status != DeliveryStatus.PendingAssignment)
            throw new InvalidDeliveryStatusForAssignmentException(Id, Status);

        AssignedAgentId = agentId;
        Status = DeliveryStatus.Assigned;

        AddDomainEvent(new DeliveryAssignedEvent(Id, agentId));
    }

    public void MarkAsPickedUp()
    {
        EnsureAssignedToAgent();

        if (Status != DeliveryStatus.Assigned)
            throw new InvalidDeliveryStatusTransitionException(Id, Status, DeliveryStatus.InTransit);

        Status = DeliveryStatus.InTransit;

        AddDomainEvent(new DeliveryPickedUpEvent(Id, AssignedAgentId.Value));
    }

    public void MarkAsCompleted()
    {
        EnsureAssignedToAgent();

        if (Status != DeliveryStatus.InTransit)
            throw new InvalidDeliveryStatusTransitionException(Id, Status, DeliveryStatus.Completed);

        Status = DeliveryStatus.Completed;

        AddDomainEvent(new DeliveryCompletedEvent(Id, AssignedAgentId.Value));
    }

    public void CancelAssignment()
    {
        if (Status != DeliveryStatus.Assigned)
            throw new InvalidDeliveryStatusTransitionException(Id, Status, DeliveryStatus.PendingAssignment);

        var previousAgentId = AssignedAgentId.Value;
        AssignedAgentId = null;
        Status = DeliveryStatus.PendingAssignment;

        AddDomainEvent(new DeliveryAssignmentCanceledEvent(Id, previousAgentId));
    }

    private void EnsureAssignedToAgent()
    {
        if (!AssignedAgentId.HasValue)
            throw new DeliveryNotAssignedToAgentException(Id);
    }
}