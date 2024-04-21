using System.Collections.Generic;
using System.Threading.Tasks;
using Cinder.Models;

namespace Cinder.Data;
/// <summary>
/// Interface for repository operations related to messages.
/// </summary>
public interface IMessageRepository
{
        /// <summary>
    /// Retrieves a conversation between two users.
    /// </summary>
    /// <param name="senderId">The ID of the sender.</param>
    /// <param name="receiverId">The ID of the receiver.</param>
    /// <returns>A collection of messages between the two users.</returns>
    Task<IEnumerable<Message>> GetConversation(string senderId, string receiverId);

    /// <summary>
    /// Adds a message to the repository.
    /// </summary>
    /// <param name="message">The message to add.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    Task AddMessage(Message message);
    
    /// <summary>
    /// Retrieves all messages for a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A collection of messages for the user.</returns>
    Task<IEnumerable<Message>> GetMessagesForUser(string userId);
}
