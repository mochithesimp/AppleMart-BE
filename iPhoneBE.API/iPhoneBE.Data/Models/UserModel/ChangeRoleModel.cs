using System.ComponentModel.DataAnnotations;

namespace iPhoneBE.Data.Models.UserModel
{
    public class ChangeRoleModel
    {
        [Required(ErrorMessage = "New role is required")]
        public string NewRole { get; set; }
    }
}