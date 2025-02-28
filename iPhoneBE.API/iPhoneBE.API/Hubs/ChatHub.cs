using iPhoneBE.Data.ViewModels.ChatDTO;
using iPhoneBE.Service.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace iPhoneBE.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatServices _chatService;
        private static readonly ConcurrentDictionary<string, string> _userConnections = new();

        public ChatHub(IChatServices chatService)
        {
            _chatService = chatService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(userId))
            {
                _userConnections.TryAdd(userId, Context.ConnectionId);
                await _chatService.UpdateUserOnlineStatus(userId, true);
                await Clients.All.SendAsync("UserOnline", userId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(userId))
            {
                _userConnections.TryRemove(userId, out _);
                await _chatService.UpdateUserOnlineStatus(userId, false);
                await Clients.All.SendAsync("UserOffline", userId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinRoom(int roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
        }

        public async Task LeaveRoom(int roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());
        }

        public async Task SendMessage(int roomId, string content)
        {
            var userId = Context.User?.Identity?.Name;
            if (string.IsNullOrEmpty(userId))
                throw new HubException("User not authenticated");

            var message = await _chatService.SendMessage(roomId, userId, content);
            await Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage", message);
        }

        public async Task MarkMessagesAsRead(int roomId)
        {
            var userId = Context.User?.Identity?.Name;
            if (string.IsNullOrEmpty(userId))
                throw new HubException("User not authenticated");

            await _chatService.MarkMessagesAsRead(roomId, userId);
            await Clients.Group(roomId.ToString()).SendAsync("MessagesRead", roomId, userId);
        }
    }
}