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
        public int Id { get; set; } // ✅ Thêm khóa chính mới

        [ForeignKey("ChatRoom")]
        public int ChatRoomID { get; set; }

        [ForeignKey("User")]
        public string UserID { get; set; } // ✅ IdentityUser.Id thường là string

        public bool IsAdmin { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual User User { get; set; }
        public virtual ChatRoom ChatRoom { get; set; }
    }

}
