using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Cinder.Models;
using Cinder.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

public class MessageRepository : IMessageRepository
{
    private readonly ApplicationContext _context;

    public MessageRepository(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Message>> GetConversation(string senderId, string receiverId)
    {
        return await _context.Messages
            .Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId) ||
                        (m.SenderId == receiverId && m.ReceiverId == senderId))
            .OrderBy(m => m.Timestamp)
            .ToListAsync();
    }

    public async Task AddMessage(Message message)
    {
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Message>> GetMessagesForUser(string userId)
    {
        return await _context.Messages
                            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                            .OrderBy(m => m.Timestamp)
                            .ToListAsync();
    }
}