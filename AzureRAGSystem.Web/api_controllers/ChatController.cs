using AzureRAGSystem.Domain.DTOs.Chat;
using AzureRAGSystem.Service.Interface;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AzureRAGSystem.Web.api_controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors] // Use default policy
public class ChatController : ControllerBase
{
    private readonly IRetrievalService _retrievalService;

    public ChatController(IRetrievalService retrievalService)
    {
        _retrievalService = retrievalService;
    }

    [HttpPost("query")]
    public async Task<IActionResult> AskQuestion([FromBody] ChatRequestDTO request)
    {
        var userId = Request.Headers["X-User-Id"].ToString() ?? "demo-user123";

        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest("The message cannot be empty");
        }

        try
        {
            var result = await _retrievalService.ProcessUserQueryAsync(userId, request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("=== CRITICAL ERROR ===");
            Console.WriteLine(ex.ToString()); 
            Console.ResetColor();
        
            return StatusCode(500, ex.Message);
        }
    }
    
    
}