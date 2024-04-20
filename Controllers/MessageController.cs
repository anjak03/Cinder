using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Cinder.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Cinder.Models;
using Cinder.Data;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;


[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IMessageRepository _messageRepository;
    private readonly MessageService _messageService;
    private readonly IHubContext<ChatHub> _hubContext; 

    public MessagesController(
        IMessageRepository messageRepository,
        IHubContext<ChatHub> hubContext) 
    {
        _messageRepository = messageRepository;
        _hubContext = hubContext; 
    }

    [HttpGet("conversation/{receiverId}")]
    public async Task<IActionResult> GetConversation(string receiverId)
    {
        var senderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var messages = await _messageRepository.GetConversation(senderId, receiverId);
        return Ok(messages);
    }

    [HttpPost("{receiverId}")]
    public async Task<IActionResult> CreateMessage(string receiverId, MessageDto messageDto)
    {
        var senderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await _messageService.AddMessageAsync(senderId, receiverId, messageDto.Content);
        await _hubContext.Clients.User(receiverId).SendAsync("ReceiveMessage", new { SenderId = senderId, Content = messageDto.Content });

        return Ok();
    }

    [HttpGet("history/{otherUserId}")]
    public async Task<IActionResult> GetChatHistory(string otherUserId)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (currentUserId == null) return Unauthorized();

        var messages = await _messageRepository.GetConversation(currentUserId, otherUserId);
        return Ok(messages);
    }

}

public class MessageDto
{
    public string ReceiverId { get; set; }
    public string Content { get; set; }
}
