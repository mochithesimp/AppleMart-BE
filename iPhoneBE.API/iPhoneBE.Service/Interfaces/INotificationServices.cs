using iPhoneBE.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace iPhoneBE.Service.Interfaces
{
    public interface INotificationServices
    {
        Task<Notification> CreateNotification(string userId, string header, string content);
        Task<List<Notification>> GetUserNotifications(string userId);
        Task<bool> MarkAsRead(int notificationId);
        Task<bool> DeleteNotification(int notificationId);
        Task<int> GetUnreadCount(string userId);
    }
}