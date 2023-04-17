using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Yottabyte.Shared.DataAnnotations;

namespace Yottabyte.Shared.Models.Events;

public class EventIM
{
    [Required]
    [Latitude]
    public double? Latitude { get; set; }

    [Required]
    [Longitude]
    public double? Longitude { get; set; }

    [Required]
    [Image]
    public IFormFile? Image { get; set; }
}
