using Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcess.Repos.IRepos
{
    public interface IChatRepository
    {
        Task<ChatMessage> SaveMessageAsync(string messageContent, string senderId, string receiverId, bool isDelivered);
        Task<List<ChatMessage>> GetMessagesAsync(string currentUserId, string otherUserId, int page, int pageSize);
        Task MarkMessagesAsDeliveredAsync(List<int> messageIds);
    }
}
