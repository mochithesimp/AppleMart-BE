using AutoMapper;
using iPhoneBE.Data.Entities;
using iPhoneBE.Data.Interfaces;
using iPhoneBE.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using iPhoneBE.Data.ViewModels.ChatVM;

namespace iPhoneBE.Service.Services
{
    public class ChatServices : IChatServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, bool> _onlineUsers;

        public ChatServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _onlineUsers = new Dictionary<string, bool>();
        }

        public async Task<List<ChatRoomViewModel>> GetUserChatRooms(string userId)
        {
            var chatRooms = await _unitOfWork.ChatRoomRepository.GetAllAsync(
                predicate: null,
                includes: new Expression<Func<ChatRoom, object>>[]
                {
                    r => r.ChatParticipants,
                    r => r.ChatMessages
                }
            );

            var userRooms = chatRooms.Where(r => r.ChatParticipants.Any(p => p.UserID == userId))
                                   .Select(room => new ChatRoomViewModel
                                   {
                                       ChatRoomID = room.ChatRoomID,
                                       RoomName = room.RoomName,
                                       IsGroup = room.IsGroup,
                                       CreatedDate = room.CreatedDate,
                                       LastMessage = _mapper.Map<ChatMessageViewModel>(
                                           room.ChatMessages.OrderByDescending(m => m.CreatedDate).FirstOrDefault()
                                       ),
                                       Participants = _mapper.Map<List<ChatParticipantViewModel>>(room.ChatParticipants)
                                   })
                                   .ToList();

            return userRooms;
        }

        public async Task<ChatRoomViewModel> GetChatRoom(int chatRoomId, int pageSize = 20, int pageNumber = 1)
        {
            var chatRoom = await _unitOfWork.ChatRoomRepository.GetByIdAsync(
                chatRoomId,
                r => r.ChatParticipants,
                r => r.ChatMessages.OrderByDescending(m => m.CreatedDate)
            );

            if (chatRoom == null)
                throw new KeyNotFoundException($"Chat room with ID {chatRoomId} not found.");

            var messages = chatRoom.ChatMessages
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(m => m.CreatedDate)
                .ToList();

            var roomViewModel = new ChatRoomViewModel
            {
                ChatRoomID = chatRoom.ChatRoomID,
                RoomName = chatRoom.RoomName,
                IsGroup = chatRoom.IsGroup,
                CreatedDate = chatRoom.CreatedDate,
                Messages = _mapper.Map<List<ChatMessageViewModel>>(messages),
                Participants = _mapper.Map<List<ChatParticipantViewModel>>(chatRoom.ChatParticipants)
            };

            return roomViewModel;
        }

        public async Task<ChatRoomViewModel> CreatePrivateRoom(string userId1, string userId2)
        {
            // Check if room already exists
            var existingRoom = await _unitOfWork.ChatRoomRepository.GetAllAsync(
                r => !r.IsGroup && r.ChatParticipants.Any(p => p.UserID == userId1)
                               && r.ChatParticipants.Any(p => p.UserID == userId2),
                r => r.ChatParticipants
            );

            if (existingRoom.Any())
                return await GetChatRoom(existingRoom.First().ChatRoomID);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var chatRoom = new ChatRoom
                {
                    RoomName = "Private Chat",
                    IsGroup = false,
                    CreatedDate = DateTime.UtcNow,
                    ChatParticipants = new List<ChatParticipant>
                    {
                        new ChatParticipant { UserID = userId1, IsAdmin = true, CreatedDate = DateTime.UtcNow },
                        new ChatParticipant { UserID = userId2, IsAdmin = true, CreatedDate = DateTime.UtcNow }
                    }
                };

                await _unitOfWork.ChatRoomRepository.AddAsync(chatRoom);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return await GetChatRoom(chatRoom.ChatRoomID);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<ChatMessageViewModel> SendMessage(int chatRoomId, string userId, string content)
        {
            var chatRoom = await _unitOfWork.ChatRoomRepository.GetByIdAsync(chatRoomId);
            if (chatRoom == null)
                throw new KeyNotFoundException($"Chat room with ID {chatRoomId} not found.");

            var message = new ChatMessage
            {
                ChatRoomID = chatRoomId,
                SenderID = int.Parse(userId),
                Content = content,
                IsRead = false,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            };

            await _unitOfWork.ChatMessageRepository.AddAsync(message);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ChatMessageViewModel>(message);
        }

        public async Task<bool> MarkMessagesAsRead(int chatRoomId, string userId)
        {
            var messages = await _unitOfWork.ChatMessageRepository.GetAllAsync(
                m => m.ChatRoomID == chatRoomId &&
                     m.SenderID.ToString() != userId &&
                     !m.IsRead
            );

            foreach (var message in messages)
            {
                message.IsRead = true;
                await _unitOfWork.ChatMessageRepository.Update(message);
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<List<ChatParticipantViewModel>> GetOnlineUsers()
        {
            var participants = await _unitOfWork.ChatParticipantRepository.GetAllAsync();
            var onlineParticipants = participants
                .Select(p => new ChatParticipantViewModel
                {
                    ID = p.ID,
                    UserID = p.UserID,
                    IsOnline = _onlineUsers.ContainsKey(p.UserID) && _onlineUsers[p.UserID]
                })
                .Where(p => p.IsOnline)
                .ToList();

            return onlineParticipants;
        }

        public async Task UpdateUserOnlineStatus(string userId, bool isOnline)
        {
            if (_onlineUsers.ContainsKey(userId))
                _onlineUsers[userId] = isOnline;
            else
                _onlineUsers.Add(userId, isOnline);
        }
    }
}