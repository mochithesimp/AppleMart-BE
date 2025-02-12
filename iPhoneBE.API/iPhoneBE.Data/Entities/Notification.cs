﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data.Model
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Header is required.")]
        [MaxLength(255, ErrorMessage = "Header cannot exceed 255 characters.")]
        public string Header { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        [MaxLength(1000, ErrorMessage = "Content cannot exceed 1000 characters.")]
        public string Content { get; set; }

        public bool IsRead { get; set; }

        public bool IsRemoved { get; set; }

        public DateTime CreatedDate { get; set; }
        public virtual User User { get; set; }
    }
}
