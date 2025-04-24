using DeliveryAssignmentEngine.Domain.Enums;

namespace DeliveryAssignmentEngine.Domain.ValueObjects;

// Package size value object
public record PackageSize
{
    public double Weight { get; }
    public double Volume { get; }

    public PackageSize(double weight, double volume)
    {
        if (weight <= 0)
            throw new ArgumentException("Weight must be positive", nameof(weight));

        if (volume <= 0)
            throw new ArgumentException("Volume must be positive", nameof(volume));

        Weight = weight;
        Volume = volume;
    }

    /// <summary>
    /// Determines if this package can be handled by a given vehicle type
    /// </summary>
    public bool CanBeHandledBy(VehicleType vehicleType)
    {
        return vehicleType switch
        {
            VehicleType.Bicycle => Weight <= 5 && Volume <= 0.02,
            VehicleType.Motorcycle => Weight <= 20 && Volume <= 0.1,
            VehicleType.Car => Weight <= 100 && Volume <= 0.5,
            VehicleType.Van => Weight <= 500 && Volume <= 2,
            VehicleType.Truck => Weight <= 2000 && Volume <= 15,
            _ => false
        };
    }
}
