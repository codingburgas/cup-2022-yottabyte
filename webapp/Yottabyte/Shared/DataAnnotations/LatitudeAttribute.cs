using System.ComponentModel.DataAnnotations;

namespace Yottabyte.Shared.DataAnnotations;

/// <summary>
/// Latitude attribute.
/// </summary>
public class LatitudeAttribute : ValidationAttribute
{
    /// <summary>
    /// Checks if the value is a valid latitude.
    /// </summary>
    /// <param name="value">Value to be cheeked.</param>
    /// <returns>Is the value valid.</returns>
    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return true;
        }

        double? latitude = value as double?;

        return latitude >= -90 && latitude <= 90;
    }
}