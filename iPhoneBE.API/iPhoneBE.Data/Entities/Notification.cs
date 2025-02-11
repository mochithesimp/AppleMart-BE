using System;
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

        [Required]
        [MaxLength(255)]
        public string Header { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; }

        public bool IsRead { get; set; }

        public bool IsRemoved { get; set; }

        public DateTime CreatedDate { get; set; }
        public virtual User User { get; set; }
    }
}
