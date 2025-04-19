using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryAssignmentEngine.Domain.Entities;

public class DeliveryAgent : Entity<DeliveryAgentId>
{
    public DeliveryAgentName Name { get; private set; }
    public DeliveryAgentStatus Status { get; private set; }
    public VehicleType VehicleType { get; private set; }
    public Location CurrentLocation { get; private set; }
    public ServiceArea ServiceArea { get; private set; }
    public Capacity Capacity { get; private set; }
    public MaxDistance MaxDistance { get; private set; }
    public IReadOnlyCollection<DeliveryId> AssignedDeliveries => _assignedDeliveries.AsReadOnly();

    private readonly List<DeliveryId> _assignedDeliveries = new();

    public DeliveryAgent(
        DeliveryAgentId id,
        DeliveryAgentName name,
        VehicleType vehicleType,
        Location startingLocation,
        ServiceArea serviceArea,
        Capacity capacity,
        MaxDistance maxDistance)
    {
        Id = id;
        Name = name;
        Status = DeliveryAgentStatus.Available;
        VehicleType = vehicleType;
        CurrentLocation = startingLocation;
        ServiceArea = serviceArea;
        Capacity = capacity;
        MaxDistance = maxDistance;
    }

    public bool IsAvailable() => Status == DeliveryAgentStatus.Available &&
                                 _assignedDeliveries.Count < Capacity.Value;

    public bool CanHandleDelivery(Delivery delivery)
    {
        return IsAvailable() &&
               ServiceArea.Contains(delivery.PickupLocation) &&
               ServiceArea.Contains(delivery.DropoffLocation) &&
               CurrentLocation.DistanceTo(delivery.PickupLocation) <= MaxDistance.Value &&
               delivery.PackageSize.CanBeHandledBy(VehicleType);
    }

    public void AssignDelivery(DeliveryId deliveryId)
    {
        if (!IsAvailable())
            throw new DeliveryAgentNotAvailableException(Id);

        _assignedDeliveries.Add(deliveryId);

        if (_assignedDeliveries.Count >= Capacity.Value)
            Status = DeliveryAgentStatus.Busy;

        AddDomainEvent(new DeliveryAssignedToAgentEvent(Id, deliveryId));
    }

    public void CompleteDelivery(DeliveryId deliveryId)
    {
        if (!_assignedDeliveries.Contains(deliveryId))
            throw new DeliveryNotAssignedToAgentException(Id, deliveryId);

        _assignedDeliveries.Remove(deliveryId);

        if (Status == DeliveryAgentStatus.Busy && _assignedDeliveries.Count < Capacity.Value)
            Status = DeliveryAgentStatus.Available;

        AddDomainEvent(new DeliveryCompletedByAgentEvent(Id, deliveryId));
    }

    public void UpdateLocation(Location newLocation)
    {
        CurrentLocation = newLocation;
        AddDomainEvent(new DeliveryAgentLocationUpdatedEvent(Id, newLocation));
    }
}
