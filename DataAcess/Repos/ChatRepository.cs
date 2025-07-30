using DataAcess.Repos.IRepos;
using Microsoft.EntityFrameworkCore;
using Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DataAcess.Repos
{
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDbContext DB;

        public ChatRepository(ApplicationDbContext _db)
        {
            DB = _db;
        }
        public async Task<List<ChatMessage>> GetMessagesAsync(string currentUserId, string otherUserId, int page, int pageSize)
        {
            return await DB.ChatMessages.Where(m =>
            (m.SenderId == currentUserId && m.ReceiverId == otherUserId) ||
            (m.SenderId == otherUserId && m.ReceiverId == currentUserId)).OrderByDescending(m => m.SentAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        }

        public async Task MarkMessagesAsDeliveredAsync(List<int> messageIds)
        {
            var messages = await DB.ChatMessages
                .Where(m => messageIds.Contains(m.Id))
                .ToListAsync();
            foreach (var message in messages) {
                message.IsDelivered = true;
            }
            await DB.SaveChangesAsync();
        }

        public async Task<ChatMessage> SaveMessageAsync(string messageContent, string senderId, string receiverId, bool isDelivered)
        {
            var chatMessage = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = messageContent,
                SentAt = DateTime.UtcNow,
                IsDelivered = isDelivered
            };
            DB.ChatMessages.Add(chatMessage);
            await DB.SaveChangesAsync();
            return chatMessage;
        }
       
        public async Task<List<ChatContact>> GetChatContactsAsync(string currentUserId)
        {
            var contacts = await DB.ChatMessages
                .Where(m => m.SenderId == currentUserId || m.ReceiverId == currentUserId)
                .Select(m => m.SenderId == currentUserId ? m.ReceiverId : m.SenderId)
                .Distinct()
                .Join(DB.ApplicationUser.Include(u => u.Image),
                      contactId => contactId,
                      user => user.Id,
                      (contactId, user) => new ChatContact
                      {
                          UserId = user.Id,
                          FullName = user.FullName,
                          ProfileImage = user.Image != null ? user.Image.FilePath : null
                      })
                .ToListAsync();

            return contacts;
        }

    }
}
