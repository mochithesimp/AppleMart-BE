using System;

namespace iPhoneBE.Data.ViewModels.ChatVM
{
    public class ChatMessageViewModel
    {
        public int ChatID { get; set; }
        public int ChatRoomID { get; set; }
        public int SenderID { get; set; }
        public string SenderName { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}