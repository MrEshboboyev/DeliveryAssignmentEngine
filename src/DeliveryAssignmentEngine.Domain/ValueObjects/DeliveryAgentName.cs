namespace DeliveryAssignmentEngine.Domain.ValueObjects;

// Named value objects
public record DeliveryAgentName
{
    public string Value { get; }

    public DeliveryAgentName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Name cannot be empty", nameof(value));

        if (value.Length > 100)
            throw new ArgumentException("Name cannot exceed 100 characters", nameof(value));

        Value = value;
    }

    public override string ToString() => Value;

    // Implicit conversion for convenience
    public static implicit operator string(DeliveryAgentName name) => name.Value;
}
