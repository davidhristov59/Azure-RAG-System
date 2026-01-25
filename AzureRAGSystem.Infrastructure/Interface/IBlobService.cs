using Microsoft.AspNetCore.Http;

namespace AzureRAGSystem.Infrastructure.Interface;

/*
 * This service handles the physical upload of files to Azure Blob Storage while attaching metadata (like userId and category) to the blob itself.
 */
public interface IBlobService
{
    Task<string> UploadFileAsync(IFormFile file, Dictionary<string, string> metadata);
    Task DeleteFileAsync(string fileName);
    string GenerateSasToken(string fileName, int expirationMinutes = 5);
}