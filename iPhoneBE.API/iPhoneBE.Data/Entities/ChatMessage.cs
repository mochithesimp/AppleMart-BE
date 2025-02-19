using iPhoneBE.Data.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data.Entities
{
    public class ChatMessage
    {
        [Key]
        public int ChatID { get; set; }

        [ForeignKey("ChatRoomID")]
        public int ChatRoomID { get; set; }

        [ForeignKey("UserID")]
        public int SenderID { get; set; }

        public string? Content { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public User User { get; set; }

        public ChatRoom ChatRoom { get; set; }
    }
}
