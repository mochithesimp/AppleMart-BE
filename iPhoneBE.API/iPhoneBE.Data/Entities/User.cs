using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iPhoneBE.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace iPhoneBE.Data.Model
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "User name is required.")]
        [MaxLength(255, ErrorMessage = "User name cannot exceed 255 characters.")]
        public string Name { get; set; }

        [MaxLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
        public string Address { get; set; }

        [MaxLength(500, ErrorMessage = "Avatar URL cannot exceed 500 characters.")]
        public string Avatar { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<Order> Orders { get; set; }
        public ICollection<Order> ShippedOrders { get; set; } //Shipper
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Review> ShippedReviews { get; set; } //Shipper
        public ICollection<Notification>? Notifications { get; set; }
        public ICollection<UserBlogView> UserBlogViews { get; set; }
        public ICollection<ChatParticipant> ChatParticipants { get; set; }

        //[Key]
        //public int UserID { get; set; }

        //[ForeignKey("Role")]
        //[Required(ErrorMessage = "Role ID is required.")]
        //public int RoleID { get; set; }

        //public virtual Role Role { get; set; }

        //[Required(ErrorMessage = "Email is required.")]
        //[MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        //[EmailAddress(ErrorMessage = "Invalid email format.")]
        //public string Email { get; set; }

        //[Required(ErrorMessage = "Password is required.")]
        //[MaxLength(255, ErrorMessage = "Password cannot exceed 255 characters.")]
        //public string Password { get; set; }

        //[MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        //[Phone(ErrorMessage = "Invalid phone number format.")]
        //public string PhoneNumber { get; set; }
    }
}
