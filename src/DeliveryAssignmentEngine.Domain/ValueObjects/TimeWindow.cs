namespace DeliveryAssignmentEngine.Domain.ValueObjects;

// Time window value object
public record TimeWindow
{
    public DateTime Start { get; }
    public DateTime End { get; }

    public TimeWindow(DateTime start, DateTime end)
    {
        if (end <= start)
            throw new ArgumentException("End time must be after start time");

        Start = start;
        End = end;
    }

    public bool Contains(DateTime time) => time >= Start && time <= End;
    public bool IsInFuture => Start > DateTime.UtcNow;
    public bool IsInPast => End < DateTime.UtcNow;
    public double DurationInHours => (End - Start).TotalHours;
    public bool Overlaps(TimeWindow other) =>
        !(End < other.Start || Start > other.End);
}
