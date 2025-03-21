using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using iPhoneBE.Service.Interfaces;
using iPhoneBE.Data.Entities;
using System.Collections.Concurrent;

namespace iPhoneBE.API.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly INotificationServices _notificationService;
        private readonly ILogger<NotificationHub> _logger;
        private static readonly ConcurrentDictionary<string, HashSet<string>> _userConnections = new();

        public NotificationHub(
            INotificationServices notificationService,
            ILogger<NotificationHub> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task LoadNotifications()
        {
            try
            {
                var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    throw new HubException("User not authenticated");
                }

                _logger.LogInformation($"Loading notifications for user {userId}");
                var notifications = await _notificationService.GetUserNotifications(userId);
                await Clients.Caller.SendAsync("LoadNotifications", notifications);

                var unreadCount = await _notificationService.GetUnreadCount(userId);
                await Clients.Caller.SendAsync("UpdateUnreadCount", unreadCount);
                _logger.LogInformation($"Loaded {notifications.Count} notifications for user {userId}, {unreadCount} unread");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading notifications");
                throw;
            }
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    _userConnections.AddOrUpdate(
                        userId,
                        new HashSet<string> { Context.ConnectionId },
                        (_, connections) =>
                        {
                            connections.Add(Context.ConnectionId);
                            return connections;
                        });

                    _logger.LogInformation($"User {userId} connected with connection ID {Context.ConnectionId}");
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");

                    await LoadNotifications();
                }
                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnConnectedAsync");
                throw;
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    if (_userConnections.TryGetValue(userId, out var connections))
                    {
                        connections.Remove(Context.ConnectionId);
                        if (connections.Count == 0)
                        {
                            _userConnections.TryRemove(userId, out _);
                        }
                    }

                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
                    _logger.LogInformation($"User {userId} disconnected from notification hub");
                }
                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnDisconnectedAsync");
                throw;
            }
        }

        public async Task SendNotification(string userId, string header, string content)
        {
            try
            {
                _logger.LogInformation($"Starting to send notification to user {userId}");

                var userConnections = GetUserConnections(userId);
                _logger.LogInformation($"User {userId} has {userConnections.Count} active connections");

                var notification = await _notificationService.CreateNotification(userId, header, content);
                _logger.LogInformation($"Created notification {notification.NotificationID} for user {userId}");

                var userGroup = $"User_{userId}";
                _logger.LogInformation($"Sending notification to group: {userGroup}");

                await Clients.Group(userGroup).SendAsync("ReceiveNotification", notification);
                _logger.LogInformation($"Successfully sent notification to group {userGroup}");

                var unreadCount = await _notificationService.GetUnreadCount(userId);
                await Clients.Group(userGroup).SendAsync("UpdateUnreadCount", unreadCount);
                _logger.LogInformation($"Updated unread count ({unreadCount}) for user {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in SendNotification for user {userId}. Error details: {ex.Message}");
                throw new HubException($"Failed to send notification: {ex.Message}");
            }
        }

        public async Task MarkAsRead(int notificationId)
        {
            try
            {
                var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    throw new HubException("User not authenticated");
                }

                var success = await _notificationService.MarkAsRead(notificationId);
                if (success)
                {
                    await Clients.Group($"User_{userId}").SendAsync("NotificationRead", notificationId);

                    var unreadCount = await _notificationService.GetUnreadCount(userId);
                    await Clients.Group($"User_{userId}").SendAsync("UpdateUnreadCount", unreadCount);

                    _logger.LogInformation($"Notification {notificationId} marked as read for user {userId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read");
                throw;
            }
        }

        public async Task DeleteNotification(int notificationId)
        {
            try
            {
                var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    throw new HubException("User not authenticated");
                }

                var success = await _notificationService.DeleteNotification(notificationId);
                if (success)
                {
                    await Clients.Group($"User_{userId}").SendAsync("NotificationDeleted", notificationId);

                    var unreadCount = await _notificationService.GetUnreadCount(userId);
                    await Clients.Group($"User_{userId}").SendAsync("UpdateUnreadCount", unreadCount);

                    _logger.LogInformation($"Notification {notificationId} deleted for user {userId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification");
                throw;
            }
        }

        public async Task Echo(string message)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation($"Echo received from user {userId}: {message}");
            await Clients.Caller.SendAsync("EchoResponse", $"Server received: {message}");
        }

        private static HashSet<string> GetUserConnections(string userId)
        {
            return _userConnections.TryGetValue(userId, out var connections) ? connections : new HashSet<string>();
        }
    }
}