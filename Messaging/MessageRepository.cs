using Cinder.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinder.Data 
{
    /// <summary>
    /// Repository for managing messages in the database.
    /// </summary>
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationContext _context;

        public MessageRepository(ApplicationContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a conversation between two users, sorted by message timestamp.
        /// </summary>
        /// <param name="senderId">The user ID of one participant in the conversation.</param>
        /// <param name="receiverId">The user ID of the other participant in the conversation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of messages.</returns>
        public async Task<IEnumerable<Message>> GetConversation(string senderId, string receiverId)
        {
            return await _context.Messages
                .Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId) ||
                            (m.SenderId == receiverId && m.ReceiverId == senderId))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }

        /// <summary>
        /// Adds a message to the database.
        /// </summary>
        /// <param name="message">The message to add.</param>
        /// <returns>A task that represents the asynchronous operation of saving the message.</returns>
        public async Task AddMessage(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
        }


        /// <summary>
        /// Retrieves all messages associated with a specific user, either sent or received, sorted by timestamp.
        /// </summary>
        /// <param name="userId">The ID of the user whose messages are to be retrieved.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of messages.</returns>
        public async Task<IEnumerable<Message>> GetMessagesForUser(string userId)
        {
            return await _context.Messages
                                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                                .OrderBy(m => m.Timestamp)
                                .ToListAsync();
        }
    }
}