namespace DeliveryAssignmentEngine.Domain.ValueObjects;

// DeliveryAgentRating value object
public record DeliveryAgentRating
{
    public double AverageRating { get; }
    public int TotalRatings { get; }

    private DeliveryAgentRating(double averageRating, int totalRatings)
    {
        AverageRating = averageRating;
        TotalRatings = totalRatings;
    }

    public static DeliveryAgentRating New() => new(0, 0);

    public DeliveryAgentRating AddRating(int rating)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5");

        if (TotalRatings == 0)
            return new DeliveryAgentRating(rating, 1);

        var newTotalRatings = TotalRatings + 1;
        var newAverageRating = (AverageRating * TotalRatings + rating) / newTotalRatings;

        return new DeliveryAgentRating(newAverageRating, newTotalRatings);
    }
}
