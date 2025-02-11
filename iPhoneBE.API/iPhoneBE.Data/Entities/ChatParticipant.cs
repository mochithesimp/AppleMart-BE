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
    public class ChatParticipant
    {
        [Key]
        public int ChatRoomID { get; set; }

        [ForeignKey("UserID")]
        public int UserID { get; set; }

        public bool IsAdmin { get; set; }

        public DateTime CreatedDate { get; set; }

        public User User { get; set; }
        public ChatRoom ChatRoom { get; set; }
    }
}
