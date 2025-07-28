using System.ComponentModel.DataAnnotations;

namespace FrontendHotelManagementSystem.Models
{
    public class Room
    {
        public int RoomId { get; set; }

        [Required]
        [Display(Name = "Room Number")]
        [StringLength(10)]
        public string RoomNumber { get; set; }

        [Required]
        [Display(Name = "Room Type ID")]
        public int RoomTypeId { get; set; }

        [Range(1, 20)]
        public int? Capacity { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Price Per Night")]
        public decimal? PricePerNight { get; set; }

        [Display(Name = "Available")]
        public bool IsAvaliable { get; set; }
        public string Status { get; internal set; }
    }
}
