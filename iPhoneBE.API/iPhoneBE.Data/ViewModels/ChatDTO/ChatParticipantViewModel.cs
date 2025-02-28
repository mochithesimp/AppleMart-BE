using System;

namespace iPhoneBE.Data.ViewModels.ChatDTO
{
    public class ChatParticipantViewModel
    {
        public int ID { get; set; }
        public int ChatRoomID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsOnline { get; set; }
    }
}