using System.ComponentModel.DataAnnotations;

namespace Yottabyte.Data.Models.Events;

public class Event
{
    [Required]
    public string? Id { get; set; }

    [Required]
    public string? Location { get; set; }

    [Required]
    public double? Latitude { get; set; }

    [Required]
    public double? Longitude { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public string? ImageURL { get; set; }
}