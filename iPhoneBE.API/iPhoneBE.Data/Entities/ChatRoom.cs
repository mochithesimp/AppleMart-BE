﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data.Entities
{
    public class ChatRoom
    {
        [Key]
        public int ChatRoomID { get; set; }

        public string RoomName { get; set; }

        public bool IsGroup { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual ICollection<ChatMessage> ChatMessages { get; set; }
        public virtual ICollection<ChatParticipant> ChatParticipants { get; set; }
    }
}
