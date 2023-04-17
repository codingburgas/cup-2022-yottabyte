using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yottabyte.Services.Contracts;

namespace Yottabyte.Services.Implementations;

/// <summary>
/// File service.
/// </summary>
internal class FileService : IFileService
{
    private readonly IHostingEnvironment environment;
    private readonly IConfiguration configuration;
    private readonly BlobServiceClient blobServiceClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileService"/> class.
    /// </summary>
    /// <param name="environment">Environment.</param>
    public FileService(IHostingEnvironment environment, IConfiguration configuration)
    {
        this.environment = environment;
        this.configuration = configuration;
        var connectionString = this.configuration["AzureStorage:ConnectionString"];
        this.blobServiceClient = new BlobServiceClient(connectionString);
    }

    /// <inheritdoc/>
    public async Task<string> SaveImageAsync(IFormFile file, string containerName)
    {
        BlobContainerClient containerClient;

        containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        containerClient.CreateIfNotExists();

        var blockBlobClient = containerClient.GetBlockBlobClient(Path.GetRandomFileName() + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName).ToLowerInvariant());

        new FileExtensionContentTypeProvider().TryGetContentType(file.FileName, out var contentType);

        var blobHttpHeader = new BlobHttpHeaders { ContentType = (contentType ?? "application/octet-stream").ToLowerInvariant() };

        await blockBlobClient.UploadAsync(
            file.OpenReadStream(),
            new BlobUploadOptions { HttpHeaders = blobHttpHeader }
        );

        return blockBlobClient.Uri.AbsoluteUri;
    }
}