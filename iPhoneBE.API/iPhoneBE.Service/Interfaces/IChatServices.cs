using iPhoneBE.Data.Entities;
using iPhoneBE.Data.ViewModels.ChatDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace iPhoneBE.Service.Interfaces
{
    public interface IChatServices
    {
        Task<List<ChatRoomViewModel>> GetUserChatRooms(string userId);
        Task<ChatRoomViewModel> GetChatRoom(int chatRoomId, int pageSize = 20, int pageNumber = 1);
        Task<ChatRoomViewModel> CreatePrivateRoom(string userId1, string userId2);
        Task<ChatMessageViewModel> SendMessage(int chatRoomId, string userId, string content);
        Task<bool> MarkMessagesAsRead(int chatRoomId, string userId);
        Task<List<ChatParticipantViewModel>> GetOnlineUsers();
        Task UpdateUserOnlineStatus(string userId, bool isOnline);
    }
}