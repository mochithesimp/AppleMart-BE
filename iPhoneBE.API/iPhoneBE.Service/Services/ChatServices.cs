using AutoMapper;
using iPhoneBE.Data.Entities;
using iPhoneBE.Data.Interfaces;
using iPhoneBE.Data.ViewModels.ChatDTO;
using iPhoneBE.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using iPhoneBE.Data.Model;
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
            try
            {
                Console.WriteLine($"Getting chat rooms for user: {userId}");

                var chatRooms = await _unitOfWork.ChatRoomRepository.GetAllAsync(
                    predicate: r => r.ChatParticipants.Any(p => p.UserID == userId),
                    includes: new Expression<Func<ChatRoom, object>>[]
                    {
                        r => r.ChatParticipants,
                        r => r.ChatMessages
                    }
                );

                var participantIds = chatRooms
                    .SelectMany(r => r.ChatParticipants.Select(p => p.UserID))
                    .Distinct()
                    .ToList();

                var users = await _unitOfWork.UserRepository
                    .GetAllAsync(u => participantIds.Contains(u.Id));

                var userDict = users.ToDictionary(u => u.Id, u => u.UserName);

                var userRooms = chatRooms.Select(room =>
                {
                    try
                    {
                        return new ChatRoomViewModel
                        {
                            ChatRoomID = room.ChatRoomID,
                            RoomName = room.RoomName ?? "Chat Room",
                            IsGroup = room.IsGroup,
                            CreatedDate = room.CreatedDate,
                            Messages = (room.ChatMessages ?? Enumerable.Empty<ChatMessage>())
                                .OrderByDescending(m => m.CreatedDate)
                                .Select(m => new ChatMessageViewModel
                                {
                                    ChatID = m.ChatMessageID,
                                    ChatRoomID = m.ChatRoomID,
                                    SenderID = m.SenderID,
                                    SenderName = userDict.TryGetValue(m.SenderID, out var senderName) ? senderName : "Unknown User",
                                    Content = m.Content ?? "",
                                    IsRead = m.IsRead,
                                    CreatedDate = m.CreatedDate,
                                    IsDeleted = m.IsDeleted
                                })
                                .ToList(),
                            Participants = (room.ChatParticipants ?? Enumerable.Empty<ChatParticipant>())
                                .Select(p => new ChatParticipantViewModel
                                {
                                    ID = p.ID,
                                    ChatRoomID = p.ChatRoomID,
                                    UserID = p.UserID,
                                    UserName = userDict.TryGetValue(p.UserID, out var userName) ? userName : "Unknown User",
                                    IsAdmin = p.IsAdmin,
                                    CreatedDate = p.CreatedDate,
                                    IsOnline = _onlineUsers.ContainsKey(p.UserID) && _onlineUsers[p.UserID]
                                })
                                .ToList()
                        };
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error mapping room {room.ChatRoomID}: {ex.Message}");
                        return null;
                    }
                })
                .Where(r => r != null)
                .ToList();

                Console.WriteLine($"Successfully mapped {userRooms.Count} rooms");
                return userRooms;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetUserChatRooms: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<ChatRoomViewModel> GetChatRoom(int chatRoomId, int pageSize = 20, int pageNumber = 1)
        {
            try
            {
                // First, get the chat room with basic includes
                var chatRoom = await _unitOfWork.ChatRoomRepository
                    .GetAllAsync(
                        predicate: r => r.ChatRoomID == chatRoomId,
                        includes: new Expression<Func<ChatRoom, object>>[]
                        {
                            r => r.ChatParticipants,
                            r => r.ChatMessages
                        }
                    );

                var room = chatRoom.FirstOrDefault();
                if (room == null)
                    throw new KeyNotFoundException($"Chat room with ID {chatRoomId} not found.");

                // Get all user IDs involved in this chat room
                var userIds = room.ChatParticipants
                    .Select(p => p.UserID)
                    .Union(room.ChatMessages.Select(m => m.SenderID))
                    .Distinct()
                    .ToList();

                // Fetch user information separately
                var users = await _unitOfWork.UserRepository
                    .GetAllAsync(u => userIds.Contains(u.Id));

                // Create a dictionary for quick user lookup
                var userDict = users.ToDictionary(u => u.Id, u => u.UserName);

                // Create the view model
                var roomViewModel = new ChatRoomViewModel
                {
                    ChatRoomID = room.ChatRoomID,
                    RoomName = room.RoomName ?? "Chat Room",
                    IsGroup = room.IsGroup,
                    CreatedDate = room.CreatedDate,
                    Messages = room.ChatMessages
                        .OrderByDescending(m => m.CreatedDate)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .Select(m => new ChatMessageViewModel
                        {
                            ChatID = m.ChatMessageID,
                            ChatRoomID = m.ChatRoomID,
                            SenderID = m.SenderID,
                            SenderName = userDict.TryGetValue(m.SenderID, out var senderName) ? senderName : "Unknown User",
                            Content = m.Content ?? "",
                            IsRead = m.IsRead,
                            CreatedDate = m.CreatedDate,
                            IsDeleted = m.IsDeleted
                        })
                        .ToList(),
                    Participants = room.ChatParticipants
                        .Select(p => new ChatParticipantViewModel
                        {
                            ID = p.ID,
                            ChatRoomID = p.ChatRoomID,
                            UserID = p.UserID,
                            UserName = userDict.TryGetValue(p.UserID, out var userName) ? userName : "Unknown User",
                            IsAdmin = p.IsAdmin,
                            CreatedDate = p.CreatedDate,
                            IsOnline = _onlineUsers.ContainsKey(p.UserID) && _onlineUsers[p.UserID]
                        })
                        .ToList()
                };

                // Set the last message
                roomViewModel.LastMessage = room.ChatMessages
                    .OrderByDescending(m => m.CreatedDate)
                    .Select(m => new ChatMessageViewModel
                    {
                        ChatID = m.ChatMessageID,
                        ChatRoomID = m.ChatRoomID,
                        SenderID = m.SenderID,
                        SenderName = userDict.TryGetValue(m.SenderID, out var senderName) ? senderName : "Unknown User",
                        Content = m.Content ?? "",
                        IsRead = m.IsRead,
                        CreatedDate = m.CreatedDate,
                        IsDeleted = m.IsDeleted
                    })
                    .FirstOrDefault();

                return roomViewModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetChatRoom: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<ChatRoomViewModel> CreatePrivateRoom(string userId1, string userId2)
        {
            Console.WriteLine($"Creating private room between {userId1} and {userId2}");

            var existingRoom = await _unitOfWork.ChatRoomRepository.GetAllAsync(
                r => !r.IsGroup && r.ChatParticipants.Any(p => p.UserID == userId1)
                               && r.ChatParticipants.Any(p => p.UserID == userId2),
                r => r.ChatParticipants
            );

            if (existingRoom.Any())
            {
                Console.WriteLine("Private room already exists.");
                return await GetChatRoom(existingRoom.First().ChatRoomID);
            }

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

                Console.WriteLine("Saving new chat room...");
                await _unitOfWork.ChatRoomRepository.AddAsync(chatRoom);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                Console.WriteLine("Chat room created successfully.");
                return await GetChatRoom(chatRoom.ChatRoomID);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                Console.WriteLine($"Error creating chat room: {ex.Message}");
                throw;
            }
        }

        public async Task<ChatMessageViewModel> SendMessage(int chatRoomId, string senderId, string content)
        {
            try
            {
                // First verify the chat room exists
                var room = await _unitOfWork.ChatRoomRepository
                    .GetSingleByConditionAsynce(r => r.ChatRoomID == chatRoomId);

                if (room == null)
                    throw new KeyNotFoundException($"Chat room with ID {chatRoomId} not found.");

                // Get sender information
                var sender = await _unitOfWork.UserRepository
                    .GetSingleByConditionAsynce(u => u.Id == senderId);

                if (sender == null)
                    throw new KeyNotFoundException($"User with ID {senderId} not found.");

                // Create and save the message
                var message = new ChatMessage
                {
                    ChatRoomID = chatRoomId,
                    SenderID = senderId,
                    Content = content,
                    IsRead = false,
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false
                };

                await _unitOfWork.ChatMessageRepository.AddAsync(message);
                await _unitOfWork.SaveChangesAsync();

                // Return the view model
                return new ChatMessageViewModel
                {
                    ChatID = message.ChatMessageID,
                    ChatRoomID = message.ChatRoomID,
                    SenderID = message.SenderID,
                    SenderName = sender.UserName,
                    Content = message.Content,
                    IsRead = message.IsRead,
                    CreatedDate = message.CreatedDate,
                    IsDeleted = message.IsDeleted
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendMessage: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
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
            var users = await _unitOfWork.UserRepository.GetAllAsync();

            return users.Select(u => new ChatParticipantViewModel
            {
                ID = 0,
                UserID = u.Id,
                UserName = u.UserName,
                IsOnline = _onlineUsers.ContainsKey(u.Id) && _onlineUsers[u.Id]
            }).ToList();
        }


        public async Task UpdateUserOnlineStatus(string userId, bool isOnline)
        {
            Console.WriteLine($"Updating Online Status: UserID={userId}, Online={isOnline}");

            if (_onlineUsers.ContainsKey(userId))
                _onlineUsers[userId] = isOnline;
            else
                _onlineUsers.Add(userId, isOnline);
        }

        public async Task<ChatRoomViewModel> CreateGroupRoom(string name, List<string> userIds)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Group name cannot be empty");

            if (userIds == null || !userIds.Any())
                throw new ArgumentException("Group must have at least one participant");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var chatRoom = new ChatRoom
                {
                    RoomName = name,
                    IsGroup = true,
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false,
                    ChatParticipants = userIds.Select(userId => new ChatParticipant
                    {
                        UserID = userId,
                        IsAdmin = true,
                        CreatedDate = DateTime.UtcNow,
                        IsDeleted = false
                    }).ToList()
                };

                await _unitOfWork.ChatRoomRepository.AddAsync(chatRoom);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return await GetChatRoom(chatRoom.ChatRoomID);
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}