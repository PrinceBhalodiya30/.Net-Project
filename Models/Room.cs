using System.ComponentModel.DataAnnotations;

namespace FrontendHotelManagementSystem.Models
{
    public class Room
    {
        public int RoomId { get; set; }
        
        [Required(ErrorMessage = "Room number is required")]
        [Display(Name = "Room Number")]
        [StringLength(10, ErrorMessage = "Room number cannot exceed 10 characters")]
        public string? RoomNumber { get; set; }
        
        [Required(ErrorMessage = "Room type is required")]
        [Display(Name = "Room Type")]
        public int RoomTypeId { get; set; }
        
        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 20, ErrorMessage = "Capacity must be between 1 and 20")]
        public int Capacity { get; set; }
        
        [Required(ErrorMessage = "Price per night is required")]
        [Display(Name = "Price Per Night")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal? PricePerNight { get; set; }
        
        [Display(Name = "Is Available")]
        public bool IsAvaliable { get; set; }
    }
}