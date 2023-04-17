namespace Yottabyte.Shared.Models.Events;

public class EventVM
{
    public string? Id { get; set; }

    public string? Location { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public DateTime StartTime { get; set; }

    public string? ImageURL { get; set; }
}