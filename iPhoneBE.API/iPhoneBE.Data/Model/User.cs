using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data.Model
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [ForeignKey("Role")]
        public int RoleID { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public string Password { get; set; }

        [MaxLength(20)]
        [Phone]
        public string PhoneNumber { get; set; }

        [MaxLength(500)]
        public string Address { get; set; }

        [MaxLength(500)]
        public string Avatar { get; set; }
        public bool IsActive { get; set; }


        // Navigation property for Role
        public virtual Role Role { get; set; }

        // Navigation property for related Order
        public ICollection<Order> Orders { get; set; }
    }
}
