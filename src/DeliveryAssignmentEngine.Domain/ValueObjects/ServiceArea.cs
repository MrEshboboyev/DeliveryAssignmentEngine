namespace DeliveryAssignmentEngine.Domain.ValueObjects;

// Service area defined as a polygon of locations
public record ServiceArea
{
    public IReadOnlyList<Location> Boundaries { get; }

    public ServiceArea(IEnumerable<Location> boundaries)
    {
        var boundaryList = boundaries?.ToList() ??
            throw new ArgumentNullException(nameof(boundaries));

        if (boundaryList.Count < 3)
            throw new ArgumentException("Service area must have at least 3 boundary points", nameof(boundaries));

        Boundaries = boundaryList.AsReadOnly();
    }

    /// <summary>
    /// Checks if a location is within this service area
    /// Uses the ray casting algorithm for point-in-polygon detection
    /// </summary>
    public bool Contains(Location location)
    {
        ArgumentNullException.ThrowIfNull(location);

        bool inside = false;
        for (int i = 0, j = Boundaries.Count - 1; i < Boundaries.Count; j = i++)
        {
            var intersect = Boundaries[i].Latitude > location.Latitude != Boundaries[j].Latitude > location.Latitude &&
                            location.Longitude < (Boundaries[j].Longitude - Boundaries[i].Longitude) *
                            (location.Latitude - Boundaries[i].Latitude) /
                            (Boundaries[j].Latitude - Boundaries[i].Latitude) + Boundaries[i].Longitude;

            if (intersect)
                inside = !inside;
        }

        return inside;
    }
}
