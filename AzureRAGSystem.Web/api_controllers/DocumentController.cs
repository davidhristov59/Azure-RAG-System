using Azure.Search.Documents;
using AzureRAGSystem.Infrastructure.Interface;
using AzureRAGSystem.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AzureRAGSystem.Web.api_controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IBlobService _blobService;
    private readonly ISearchIndexerService _searchIndexerService;

    public DocumentsController(
        IBlobService blobService, 
        ISearchIndexerService searchIndexerService)
    {
        _blobService = blobService;
        _searchIndexerService = searchIndexerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDocuments()
    {
        var userId = Request.Headers["X-User-Id"].ToString();
        if (string.IsNullOrWhiteSpace(userId))
        {
            userId = "david123";
        }

        try
        {
            var options = new SearchOptions
            {
                // Filter = $"userId eq {userId}"
                Filter = $"userId eq null", // Partition-level security
            };
            
            options.Select.Add("id");
            options.Select.Add("title");
            options.Select.Add("category");
            options.Select.Add("sourceUrl");
            options.Select.Add("content");
            options.Select.Add("userId");

            var response = await _searchIndexerService.SearchDocumentsAsync("*", options); // perform a "match-all" search within the user's partition

            return Ok(response);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Internal Server Error: {e.Message}");
        }
    }
    
    [HttpGet("indexer-status")]
    public async Task<IActionResult> GetIndexerStatus() // to expose the indexer status to the UI
    {
        var status = await _searchIndexerService.GetIndexerStatusAsync();
        return Ok(new { Status = status });
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file, [FromForm] string category)
    {
        var userId = Request.Headers["X-User-Id"].ToString();

        if (string.IsNullOrWhiteSpace(userId))
        {
            userId = "david123";
        }

        // upload to Blob Storage with Metadata
        var metadata = new Dictionary<string, string> 
        { 
            { "category", category }, 
            { "userId", userId } 
        };
        var blobUrl = await _blobService.UploadFileAsync(file, metadata);

        // trigger the Search Indexer
        await _searchIndexerService.RunIndexerAsync();

        return Ok(new
        {
            Url = blobUrl, 
            Message = "File uploaded and indexing started."
        });
    }

    [HttpDelete("{fileName}")]
    public async Task<IActionResult> Delete(string fileName)
    {
        var userId = Request.Headers["X-User-Id"].ToString();
        
        if (string.IsNullOrWhiteSpace(userId))
        {
            userId = "david123";
        }
        
        // delete from Blob
        await _blobService.DeleteFileAsync(fileName);

        // delete from Search Index using the encoded path as Key
        await _searchIndexerService.DeleteDocumentFromIndexAsync(fileName);

        return Ok("Document removed from storage and index.");
    }

    [HttpGet("preview/{fileName}")]
    public IActionResult GetDocumentPreviewUrl(string fileName)
    {
        var sasUrl = _blobService.GenerateSasToken(fileName);
        return Ok(new { Url = sasUrl });
    }
}