using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using AzureRAGSystem.Infrastructure.Interface;
using Microsoft.AspNetCore.Http;

namespace AzureRAGSystem.Infrastructure.Implementation;

public class BlobService : IBlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName = "documents-is";

    public BlobService(string connectionString)
    {
        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<string> UploadFileAsync(IFormFile file, Dictionary<string, string> metadata)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        await containerClient.CreateIfNotExistsAsync(publicAccessType: PublicAccessType.None);

        var blobClient = containerClient.GetBlobClient(file.FileName);
        
        // Upload with metadata in a single request
        var uploadOptions = new BlobUploadOptions()
        {
            Metadata = metadata,
            HttpHeaders = new BlobHttpHeaders()
            {
                ContentType = file.ContentType
            }
        };
        
        using var stream = file.OpenReadStream(); // await
        await blobClient.UploadAsync(stream, uploadOptions);

        return blobClient.Uri.ToString();
    }

    public async Task DeleteFileAsync(string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        var blobClient = containerClient.GetBlobClient(fileName);
        await blobClient.DeleteIfExistsAsync();
    }

    public string GenerateSasToken(string fileName, int expirationMinutes = 5)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        if (!blobClient.CanGenerateSasUri)
        {
            return string.Empty;
        }

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerName,
            BlobName = fileName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expirationMinutes)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        return blobClient.GenerateSasUri(sasBuilder).ToString();
    }
}