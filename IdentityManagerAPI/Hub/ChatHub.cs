using DataAcess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Models.Domain;
using StackExchange.Redis;

namespace IdentityManagerAPI
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IConnectionMultiplexer redis;
        private readonly ApplicationDbContext DB;

        public ChatHub(IConnectionMultiplexer _redis, ApplicationDbContext _db)
        {
            redis = _redis;
            DB = _db;
        }
        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (userId != null)
            {
                var db = redis.GetDatabase();
                await db.StringSetAsync($"chat:user:{userId}:conn", Context.ConnectionId);
            }
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            if (userId != null)
            {
                var db = redis.GetDatabase();
                await db.KeyDeleteAsync($"chat:user:{userId}:conn");
            }
            await base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessage(string toUserId, string messageContent)
        {
            var senderId = Context.UserIdentifier;
            if (senderId == null || string.IsNullOrEmpty(messageContent))
                return;
            var db = redis.GetDatabase();
            var connectionId = await db.StringGetAsync($"chat:user:{toUserId}:conn");
            var chatMessage = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = toUserId,
                Content = messageContent,
                SentAt = DateTime.UtcNow,
                IsDelivered = !connectionId.IsNullOrEmpty
            };
            DB.ChatMessages.Add(chatMessage);
            await DB.SaveChangesAsync();
            if(!connectionId.IsNullOrEmpty)
                await Clients.Client(connectionId!).SendAsync("ReceiveMessage", senderId, messageContent, chatMessage.SentAt);
        }
    }
    
}
