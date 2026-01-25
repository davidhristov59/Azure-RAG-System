using AzureRAGSystem.Domain.DTOs.Chat;
using AzureRAGSystem.Domain.DTOs.Session;
using AzureRAGSystem.Repository.Interface;
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
    private readonly ICosmosdbRepository _cosmosdbRepository;

    public ChatController(IRetrievalService retrievalService, ICosmosdbRepository cosmosdbRepository)
    {
        _retrievalService = retrievalService;
        _cosmosdbRepository = cosmosdbRepository;
    }

    [HttpPost("query")]
    public async Task<IActionResult> AskQuestion([FromBody] ChatRequestDTO request)
    {
        var userId = Request.Headers["X-User-Id"].ToString();

        if (string.IsNullOrWhiteSpace(userId)) 
        {
            userId = "david123";
        }
        
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
    
    [HttpGet("sessions")]
    public async Task<IActionResult> GetUserSessions() // get all sessions for a user
    {
        try
        {
            var userId = Request.Headers["X-User-Id"].ToString();

            if (string.IsNullOrWhiteSpace(userId))
            {
                userId = "david123";
            }
            
            // fetch the sessions from my repository (CosmosDB)
            var sessions = await _cosmosdbRepository.GetUserSessionsAsync(userId);

            var sessionList = sessions.Select(s => new UserSessionDTO(
                s.Id, 
                s.Title ?? "New Chat",
                s.CreatedAt,
                userId
            ));
            return Ok(sessionList);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error retrieving user sessions");
        }
    }
    
    [HttpGet("history/{sessionId}")] 
    public async Task<IActionResult> GetChatHistory(Guid sessionId) // get all messages for a specific session
    {
        try
        {
            var userId = Request.Headers["X-User-Id"].ToString();
            
            if (string.IsNullOrWhiteSpace(userId))
            {
                userId = "david123";
            }
            
            // fetch the session from my repository (CosmosDB)
            var session = await _cosmosdbRepository.GetSessionByIdAsync(sessionId.ToString(), userId);
        
            if (session == null) return NotFound($"Session {sessionId} not found");
        
            return Ok(session.Messages);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error retrieving chat history");
        }
    }
}