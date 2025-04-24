namespace DeliveryAssignmentEngine.Domain.ValueObjects;

// Capacity value object
public record Capacity
{
    public int Value { get; }

    public Capacity(int value)
    {
        if (value <= 0)
            throw new ArgumentException("Capacity must be positive", nameof(value));

        Value = value;
    }

    public static implicit operator int(Capacity capacity) => capacity.Value;
}
