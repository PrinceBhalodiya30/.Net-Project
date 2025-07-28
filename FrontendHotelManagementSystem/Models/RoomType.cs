using System.ComponentModel.DataAnnotations;

namespace FrontendHotelManagementSystem.Models
{
    public class RoomType
    {
        public int RoomTypeId { get; set; }

        [Required]
        [Display(Name = "Room Type Name")]
        [StringLength(50)]
        public string RoomTypeName { get; set; }
    }
}
