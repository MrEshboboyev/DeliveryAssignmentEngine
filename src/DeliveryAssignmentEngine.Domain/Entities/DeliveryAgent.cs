using DeliveryAssignmentEngine.Domain.Core;
using DeliveryAssignmentEngine.Domain.Enums;
using DeliveryAssignmentEngine.Domain.Events;
using DeliveryAssignmentEngine.Domain.ValueObjects;

namespace DeliveryAssignmentEngine.Domain.Entities;

/// <summary>
/// DeliveryAgent entity represents a delivery person who can be assigned to deliveries
/// </summary>
public class DeliveryAgent : Entity<DeliveryAgentId>
{
    // Properties
    public DeliveryAgentName Name { get; private set; }
    public DeliveryAgentStatus Status { get; private set; }
    public VehicleType VehicleType { get; private set; }
    public Location CurrentLocation { get; private set; }
    public ServiceArea ServiceArea { get; private set; }
    public Capacity Capacity { get; private set; }
    public MaxDistance MaxDistance { get; private set; }
    public DeliveryAgentRating Rating { get; private set; }

    // Collection navigation property for assigned deliveries
    private readonly List<DeliveryId> _assignedDeliveries = [];
    public IReadOnlyCollection<DeliveryId> AssignedDeliveries => _assignedDeliveries.AsReadOnly();

    // For EF Core
    private DeliveryAgent() { }

    public DeliveryAgent(
        DeliveryAgentId id,
        DeliveryAgentName name,
        VehicleType vehicleType,
        Location startingLocation,
        ServiceArea serviceArea,
        Capacity capacity,
        MaxDistance maxDistance) : base(id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        VehicleType = vehicleType;
        CurrentLocation = startingLocation ?? throw new ArgumentNullException(nameof(startingLocation));
        ServiceArea = serviceArea ?? throw new ArgumentNullException(nameof(serviceArea));
        Capacity = capacity ?? throw new ArgumentNullException(nameof(capacity));
        MaxDistance = maxDistance ?? throw new ArgumentNullException(nameof(maxDistance));
        Status = DeliveryAgentStatus.Available;
        Rating = DeliveryAgentRating.New(); // Initialize with default rating

        AddDomainEvent(new DeliveryAgentCreatedEvent(id));
    }

    // Domain behaviors

    /// <summary>
    /// Checks if the agent is available to take new deliveries
    /// </summary>
    public bool IsAvailable() =>
        Status == DeliveryAgentStatus.Available &&
        _assignedDeliveries.Count < Capacity.Value;

    /// <summary>
    /// Determines if the agent can handle a specific delivery based on various criteria
    /// </summary>
    public bool CanHandleDelivery(Delivery delivery)
    {
        ArgumentNullException.ThrowIfNull(delivery);

        // Check availability
        if (!IsAvailable())
            return false;

        // Check service area
        if (!ServiceArea.Contains(delivery.PickupLocation) ||
            !ServiceArea.Contains(delivery.DropoffLocation))
            return false;

        // Check distance
        if (CurrentLocation.DistanceTo(delivery.PickupLocation) > MaxDistance.Value)
            return false;

        // Check package handling capability
        if (!delivery.PackageSize.CanBeHandledBy(VehicleType))
            return false;

        // Check delivery time window compatibility
        // Could add more sophisticated time window checking based on current assignments

        return true;
    }

    /// <summary>
    /// Assigns a delivery to this agent
    /// </summary>
    public void AssignDelivery(DeliveryId deliveryId)
    {
        if (!IsAvailable())
            throw new DeliveryAgentNotAvailableException(Id);

        _assignedDeliveries.Add(deliveryId);

        // Update status if capacity reached
        if (_assignedDeliveries.Count >= Capacity.Value)
            Status = DeliveryAgentStatus.Busy;

        AddDomainEvent(new DeliveryAssignedToAgentEvent(Id, deliveryId));
    }

    /// <summary>
    /// Marks a delivery as completed and removes it from the agent's assignments
    /// </summary>
    public void CompleteDelivery(DeliveryId deliveryId)
    {
        if (!_assignedDeliveries.Contains(deliveryId))
            throw new DeliveryNotAssignedToAgentException(Id, deliveryId);

        _assignedDeliveries.Remove(deliveryId);

        // Update status if was at full capacity and now has room
        if (Status == DeliveryAgentStatus.Busy && _assignedDeliveries.Count < Capacity.Value)
            Status = DeliveryAgentStatus.Available;

        AddDomainEvent(new DeliveryCompletedByAgentEvent(Id, deliveryId));
    }

    /// <summary>
    /// Updates the agent's current location
    /// </summary>
    public void UpdateLocation(Location newLocation)
    {
        ArgumentNullException.ThrowIfNull(newLocation);

        CurrentLocation = newLocation;
        AddDomainEvent(new DeliveryAgentLocationUpdatedEvent(Id, newLocation));
    }

    /// <summary>
    /// Updates the agent's status
    /// </summary>
    public void UpdateStatus(DeliveryAgentStatus newStatus)
    {
        // Validate status transition
        if (newStatus == DeliveryAgentStatus.Busy && _assignedDeliveries.Count == 0)
            throw new InvalidStatusTransitionException(
                $"Cannot transition to {nameof(DeliveryAgentStatus.Busy)} with no assigned deliveries");

        Status = newStatus;
        AddDomainEvent(new DeliveryAgentStatusUpdatedEvent(Id, newStatus));
    }

    /// <summary>
    /// Updates the agent's rating based on a new delivery rating
    /// </summary>
    public void UpdateRating(int deliveryRating)
    {
        Rating = Rating.AddRating(deliveryRating);
        AddDomainEvent(new DeliveryAgentRatingUpdatedEvent(Id, Rating));
    }
}
