using Microsoft.AspNetCore.Http;

namespace Yottabyte.Services.Contracts;

/// <summary>
/// Interface of the file service.
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Saves image to the server.
    /// </summary>
    /// <param name="file">Image.</param>
    /// <param name="directory">Directory for the image.</param>
    /// <returns>URL of the image.</returns>
    Task<string> SaveImageAsync(IFormFile file, string directory);
}