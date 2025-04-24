using DeliveryAssignmentEngine.Domain.Core;
using DeliveryAssignmentEngine.Domain.Enums;
using DeliveryAssignmentEngine.Domain.Events;
using DeliveryAssignmentEngine.Domain.ValueObjects;

namespace DeliveryAssignmentEngine.Domain.Entities;

/// <summary>
/// Delivery aggregate root represents a delivery request with its complete lifecycle
/// </summary>
public class Delivery : AggregateRoot<DeliveryId>
{
    // Properties
    public DeliveryStatus Status { get; private set; }
    public CustomerId CustomerId { get; private set; }
    public Location PickupLocation { get; private set; }
    public Location DropoffLocation { get; private set; }
    public TimeWindow DeliveryWindow { get; private set; }
    public PackageSize PackageSize { get; private set; }
    public DeliveryAgentId AssignedAgentId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DeliveryPriority Priority { get; private set; }
    public DateTime? PickupTime { get; private set; }
    public DateTime? DeliveryTime { get; private set; }

    // For EF Core
    private Delivery() { }

    public Delivery(
        DeliveryId id,
        CustomerId customerId,
        Location pickupLocation,
        Location dropoffLocation,
        TimeWindow deliveryWindow,
        PackageSize packageSize,
        DeliveryPriority priority = DeliveryPriority.Standard) : base(id)
    {
        // Validation
        CustomerId = customerId ?? throw new ArgumentNullException(nameof(customerId));
        PickupLocation = pickupLocation ?? throw new ArgumentNullException(nameof(pickupLocation));
        DropoffLocation = dropoffLocation ?? throw new ArgumentNullException(nameof(dropoffLocation));
        DeliveryWindow = deliveryWindow ?? throw new ArgumentNullException(nameof(deliveryWindow));
        PackageSize = packageSize ?? throw new ArgumentNullException(nameof(packageSize));
        Priority = priority;

        // Initialize state
        Status = DeliveryStatus.Created;
        CreatedAt = DateTime.UtcNow;

        // Raise domain event
        AddDomainEvent(new DeliveryCreatedEvent(id, customerId));
    }

    // Domain behaviors

    /// <summary>
    /// Assigns this delivery to a delivery agent
    /// </summary>
    public void AssignToAgent(DeliveryAgentId agentId)
    {
        if (Status != DeliveryStatus.Created && Status != DeliveryStatus.PendingAssignment)
            throw new InvalidDeliveryStatusForAssignmentException(Id, Status);

        AssignedAgentId = agentId ?? throw new ArgumentNullException(nameof(agentId));
        Status = DeliveryStatus.Assigned;

        AddDomainEvent(new DeliveryAssignedEvent(Id, agentId));
    }

    /// <summary>
    /// Marks delivery as picked up by the assigned agent
    /// </summary>
    public void MarkAsPickedUp()
    {
        EnsureAssignedToAgent();

        if (Status != DeliveryStatus.Assigned)
            throw new InvalidDeliveryStatusTransitionException(Id, Status, DeliveryStatus.InTransit);

        Status = DeliveryStatus.InTransit;
        PickupTime = DateTime.UtcNow;

        AddDomainEvent(new DeliveryPickedUpEvent(Id, AssignedAgentId!.Value));
    }

    /// <summary>
    /// Marks delivery as completed (delivered to customer)
    /// </summary>
    public void MarkAsCompleted()
    {
        EnsureAssignedToAgent();

        if (Status != DeliveryStatus.InTransit)
            throw new InvalidDeliveryStatusTransitionException(Id, Status, DeliveryStatus.Completed);

        Status = DeliveryStatus.Completed;
        DeliveryTime = DateTime.UtcNow;

        AddDomainEvent(new DeliveryCompletedEvent(Id, AssignedAgentId!.Value));
    }

    /// <summary>
    /// Cancels an assignment, putting the delivery back into pending status
    /// </summary>
    public void CancelAssignment()
    {
        if (Status != DeliveryStatus.Assigned)
            throw new InvalidDeliveryStatusTransitionException(Id, Status, DeliveryStatus.PendingAssignment);

        var previousAgentId = AssignedAgentId!.Value;
        AssignedAgentId = null;
        Status = DeliveryStatus.PendingAssignment;

        AddDomainEvent(new DeliveryAssignmentCanceledEvent(Id, previousAgentId));
    }

    /// <summary>
    /// Marks delivery as failed
    /// </summary>
    public void MarkAsFailed(string reason)
    {
        if (Status == DeliveryStatus.Completed || Status == DeliveryStatus.Canceled)
            throw new InvalidDeliveryStatusTransitionException(Id, Status, DeliveryStatus.Failed);

        Status = DeliveryStatus.Failed;

        AddDomainEvent(new DeliveryFailedEvent(Id, AssignedAgentId, reason));
    }

    /// <summary>
    /// Cancels the delivery
    /// </summary>
    public void Cancel(string reason)
    {
        if (Status == DeliveryStatus.Completed || Status == DeliveryStatus.Failed)
            throw new InvalidDeliveryStatusTransitionException(Id, Status, DeliveryStatus.Canceled);

        Status = DeliveryStatus.Canceled;

        AddDomainEvent(new DeliveryCanceledEvent(Id, AssignedAgentId, reason));
    }

    /// <summary>
    /// Updates the delivery priority
    /// </summary>
    public void UpdatePriority(DeliveryPriority priority)
    {
        Priority = priority;
        AddDomainEvent(new DeliveryPriorityUpdatedEvent(Id, priority));
    }

    /// <summary>
    /// Checks if the delivery is eligible for assignment
    /// </summary>
    public bool IsEligibleForAssignment()
    {
        return Status == DeliveryStatus.Created || Status == DeliveryStatus.PendingAssignment;
    }

    /// <summary>
    /// Helper to ensure delivery is assigned to an agent before operation
    /// </summary>
    private void EnsureAssignedToAgent()
    {
        if (!AssignedAgentId.HasValue)
            throw new DeliveryNotAssignedToAgentException(Id);
    }

    /// <summary>
    /// Apply method for event sourcing (if utilized)
    /// </summary>
    private void Apply(DeliveryAssignedEvent @event)
    {
        AssignedAgentId = @event.AgentId;
        Status = DeliveryStatus.Assigned;
    }

    /// <summary>
    /// Apply method for event sourcing (if utilized)
    /// </summary>
    private void Apply(DeliveryPickedUpEvent @event)
    {
        Status = DeliveryStatus.InTransit;
        PickupTime = @event.OccurredOn;
    }

    /// <summary>
    /// Apply method for event sourcing (if utilized)
    /// </summary>
    private void Apply(DeliveryCompletedEvent @event)
    {
        Status = DeliveryStatus.Completed;
        DeliveryTime = @event.OccurredOn;
    }
}
