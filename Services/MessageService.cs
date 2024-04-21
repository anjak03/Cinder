using Cinder.Data;
using Cinder.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Service responsible for handling message-related operations, such as adding messages and retrieving conversations.
/// </summary>
public class MessageService
{
    private readonly ApplicationContext _context;

    public MessageService(ApplicationContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adds a message to the database asynchronously.
    /// </summary>
    /// <param name="senderId">The ID of the user sending the message.</param>
    /// <param name="receiverId">The ID of the user receiving the message.</param>
    /// <param name="content">The content of the message.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddMessageAsync(string senderId, string receiverId, string content)
    {
        var message = new Message
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = content,
            Timestamp = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves a conversation between two users sorted by the timestamp of the messages.
    /// </summary>
    /// <param name="userId1">The ID of the first user in the conversation.</param>
    /// <param name="userId2">The ID of the second user in the conversation.</param>
    /// <returns>A task that returns a list of messages representing the conversation between the two users.</returns>
    public async Task<IEnumerable<Message>> GetConversationAsync(string userId1, string userId2)
    {
        return await _context.Messages
            .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                        (m.SenderId == userId2 && m.ReceiverId == userId1))
            .OrderBy(m => m.Timestamp) 
            .ToListAsync();
    }

    /// <summary>
    /// Convenience method to add a message to the chat. Acts as a wrapper over AddMessageAsync.
    /// </summary>
    /// <param name="senderId">The ID of the user sending the message.</param>
    /// <param name="receiverId">The ID of the user receiving the message.</param>
    /// <param name="content">The content of the message.</param>
    /// <returns>A task representing the asynchronous operation of sending the message.</returns>
    public async Task AddMessageToChat(string senderId, string receiverId, string content)
    {
        await AddMessageAsync(senderId, receiverId, content);
    }

}
