using System.ComponentModel.DataAnnotations;

namespace Yottabyte.Shared.DataAnnotations;

/// <summary>
/// Longitude attribute.
/// </summary>
public class LongitudeAttribute : ValidationAttribute
{
    /// <summary>
    /// Checks if the value is a valid longitude.
    /// </summary>
    /// <param name="value">Value to be checked.</param>
    /// <returns>Is the value a longitude.</returns>
    public override bool IsValid(object? value)
    {
        if (value == null)
        {
            return true;
        }

        double? longitude = value as double?;

        return longitude >= -180 && longitude <= 180;
    }
}