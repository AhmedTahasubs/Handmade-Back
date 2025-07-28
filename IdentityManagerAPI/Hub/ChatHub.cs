using DataAcess;
using DataAcess.Repos.IRepos;
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
        private readonly IChatRepository chatRepo;

        public ChatHub(IConnectionMultiplexer _redis, IChatRepository _chatRepo)
        {
            redis = _redis;
            chatRepo = _chatRepo;
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

            var chatMessage = await chatRepo.SaveMessageAsync(messageContent, senderId, toUserId, !connectionId.IsNullOrEmpty);

            // Send to recipient if online
            if (!connectionId.IsNullOrEmpty)
            {
                await Clients.Client(connectionId!).SendAsync("ReceiveMessage", chatMessage);
            }

            // Always send to sender
            await Clients.User(senderId).SendAsync("ReceiveMessage", chatMessage);
        }

    }

}
