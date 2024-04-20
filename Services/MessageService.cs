using Cinder.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Cinder.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Diagnostics;


public class MessageService
{
    private readonly ApplicationContext _context;

    public MessageService(ApplicationContext context)
    {
        _context = context;
    }

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


    public async Task<IEnumerable<Message>> GetConversationAsync(string userId1, string userId2)
    {
        return await _context.Messages
            .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                        (m.SenderId == userId2 && m.ReceiverId == userId1))
            .OrderBy(m => m.Timestamp) 
            .ToListAsync();
    }

    public async Task AddMessageToChat(string senderId, string receiverId, string content)
    {
        await AddMessageAsync(senderId, receiverId, content);
    }

}
