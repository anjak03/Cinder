using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Cinder.Models;
using Cinder.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

public interface IMessageRepository
{
    Task<IEnumerable<Message>> GetConversation(string senderId, string receiverId);
    Task AddMessage(Message message);
    Task<IEnumerable<Message>> GetMessagesForUser(string userId);
}
