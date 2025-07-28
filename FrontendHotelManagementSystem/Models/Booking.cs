using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FrontendHotelManagementSystem.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Customer ID is required")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Room ID is required")]
        public int RoomId { get; set; }

        [Display(Name = "ID Proof Type")]
        [StringLength(50)]
        public string? IdproofType { get; set; }

        [Display(Name = "ID Proof No")]
        [StringLength(50)]
        public string? IdproofTypeNo { get; set; }

        [Display(Name = "ID Proof URL")]
        [StringLength(200)]
        public string? IdproofUrl { get; set; }

        [Display(Name = "Check In Date")]
        [DataType(DataType.Date)]
        public DateTime? CheckInDate { get; set; }

        [Display(Name = "Check Out Date")]
        [DataType(DataType.Date)]
        public DateTime? CheckOutDate { get; set; }

        [Range(1, 20)]
        [Display(Name = "No. of Guests")]
        public int? NumberOfGuests { get; set; }

        [Range(0, 10)]
        [Display(Name = "No. of Children")]
        public int? NumberOfChildrens { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Total Amount")]
        public decimal? TotalAmount { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Discount Amount")]
        public decimal? DiscountAmount { get; set; }

        [StringLength(20)]
        [Display(Name = "Booking Status")]
        public string? BookingStatus { get; set; }
    }
}
