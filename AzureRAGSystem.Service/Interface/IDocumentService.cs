using AzureRAGSystem.Domain.DomainModels;
using Microsoft.AspNetCore.Components.Forms;

namespace AzureRAGSystem.Service.Interface;

public interface IDocumentService
{
    Task<string> UploadDocumentAsync(IBrowserFile file, string category);
    Task<List<DocumentModel>> GetDocumentsAsync();
    Task<bool> DeleteDocumentAsync(string fileName);
    Task<string> GetDocumentPreviewUrlAsync(string fileName);
}