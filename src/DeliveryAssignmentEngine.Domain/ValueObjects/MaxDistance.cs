namespace DeliveryAssignmentEngine.Domain.ValueObjects;

// MaxDistance value object
public record MaxDistance
{
    public double Value { get; }

    public MaxDistance(double kilometerValue)
    {
        if (kilometerValue <= 0)
            throw new ArgumentException("Max distance must be positive", nameof(kilometerValue));

        Value = kilometerValue;
    }

    public static implicit operator double(MaxDistance distance) => distance.Value;
}
