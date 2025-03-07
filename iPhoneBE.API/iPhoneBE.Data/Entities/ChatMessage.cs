using iPhoneBE.Data.Interfaces;
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
    public class ChatMessage : IBaseEntity
    {
        [Key]
        public int ChatMessageID { get; set; }

        [ForeignKey("ChatRoomID")]
        public int ChatRoomID { get; set; }

        public string SenderID { get; set; }

        public string Content { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public virtual ChatRoom ChatRoom { get; set; }
        public virtual User User { get; set; }
    }
}
