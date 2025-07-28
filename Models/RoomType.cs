using System.ComponentModel.DataAnnotations;

namespace FrontendHotelManagementSystem.Models
{
    public class RoomType
    {
        public int RoomTypeId { get; set; }
        
        [Required(ErrorMessage = "Room type name is required")]
        [Display(Name = "Room Type Name")]
        [StringLength(50, ErrorMessage = "Room type name cannot exceed 50 characters")]
        public string? RoomTypeName { get; set; }
    }
}