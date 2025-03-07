using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace iPhoneBE.Data.ViewModels.ChatDTO
{
    public class CreateGroupRoomRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; }

        [Required]
        [MinLength(1)]
        public List<string> Participants { get; set; }
    }
}