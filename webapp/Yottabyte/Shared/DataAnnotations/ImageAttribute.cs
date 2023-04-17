using ImageMagick;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace Yottabyte.Shared.DataAnnotations;

/// <summary>
/// Image attribute.
/// </summary>
public class ImageAttribute : ValidationAttribute
{
    /// <summary>
    /// Check if the file uploaded is a valid image.
    /// </summary>
    /// <param name="value">The file.</param>
    /// <returns>Is the file an image.</returns>
    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return true;
        }

        if (value is not IFormFile image)
        {
            return false;
        }

        try
        {
            using MagickImage magickImage = new(image.OpenReadStream());
            return true;
        }
        catch
        {
            return false;
        }
    }
}