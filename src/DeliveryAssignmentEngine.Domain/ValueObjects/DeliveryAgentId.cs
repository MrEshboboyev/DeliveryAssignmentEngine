using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryAssignmentEngine.Domain.ValueObjects;

public record DeliveryAgentId(Guid Value);
public record DeliveryId(Guid Value);
public record CustomerId(Guid Value);
public record DeliveryAgentName(string Value);
public record Location(double Latitude, double Longitude)
{
    public double DistanceTo(Location other)
    {
        // Haversine formula implementation for Earth distance calculation
        const double earthRadiusKm = 6371.0;
        var dLat = ToRadians(other.Latitude - Latitude);
        var dLon = ToRadians(other.Longitude - Longitude);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(Latitude)) * Math.Cos(ToRadians(other.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadiusKm * c;
    }

    private double ToRadians(double degrees) => degrees * Math.PI / 180.0;
}

public record ServiceArea
{
    public IReadOnlyCollection<Location> Boundaries { get; }

    public ServiceArea(IEnumerable<Location> boundaries)
    {
        Boundaries = boundaries.ToList().AsReadOnly();
    }

    public bool Contains(Location location)
    {
        // Point-in-polygon algorithm implementation
        // This is simplified; a real implementation would use geospatial libraries
        return true; // Placeholder
    }
}

public record TimeWindow(DateTime Start, DateTime End)
{
    public bool Contains(DateTime time) => time >= Start && time <= End;
    public bool IsInFuture => Start > DateTime.UtcNow;
    public bool IsInPast => End < DateTime.UtcNow;
    public double DurationInHours => (End - Start).TotalHours;
}

public record Capacity(int Value);
public record MaxDistance(double KilometerValue);

public record PackageSize(double Weight, double Volume)
{
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