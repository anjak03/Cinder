using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Cinder.Models;
using System;
using Cinder.Data;
using Microsoft.EntityFrameworkCore;

public class ChatHub : Hub
{
    private readonly MessageService _messageService;
    private readonly ApplicationContext _context; 

    public ChatHub(MessageService messageService, ApplicationContext context)
    {
        _messageService = messageService;
        _context = context;
    }

    public async Task SendMessageToUser(string receiverId, string message)
    {
        var senderId = Context.UserIdentifier; 

        var sender = await _context.Users
            .Include(u => u.MatchedUsers)
            .FirstOrDefaultAsync(u => u.Id == senderId);

        if (sender != null && sender.MatchedUsers.Any(u => u.Id == receiverId))
        {
            var receiver = await _context.Users.FindAsync(receiverId);
            if (receiver != null)
            {
                await _messageService.AddMessageAsync(senderId, receiverId, message);
                await Clients.User(receiverId).SendAsync("ReceiveMessage", new { SenderId = senderId, Content = message });
            }
        }
        else
        {
            await Clients.User(senderId).SendAsync("ErrorMessage", "You can only send messages to matched users.");
        }
    }

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"User connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        Console.WriteLine($"User disconnected: {Context.ConnectionId}");
        await base.OnDisconnectedAsync(exception);
    }
}