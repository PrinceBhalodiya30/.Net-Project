using System.ComponentModel.DataAnnotations;

namespace ClientHotelSystem.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        
        [Required(ErrorMessage = "Customer is required")]
        public int CustomerId { get; set; }
        
        [Required(ErrorMessage = "Room is required")]
        public int RoomId { get; set; }
        
        [Required(ErrorMessage = "ID Proof Type is required")]
        [Display(Name = "ID Proof Type")]
        public string? IdproofType { get; set; }
        
        [Required(ErrorMessage = "ID Proof Number is required")]
        [Display(Name = "ID Proof Number")]
        public string? IdproofTypeNo { get; set; }
        
        [Display(Name = "ID Proof Document URL")]
        public string? IdproofUrl { get; set; }
        
        [Required(ErrorMessage = "Check-in date is required")]
        [Display(Name = "Check-in Date")]
        [DataType(DataType.Date)]
        public DateTime? CheckInDate { get; set; }
        
        [Required(ErrorMessage = "Check-out date is required")]
        [Display(Name = "Check-out Date")]
        [DataType(DataType.Date)]
        public DateTime? CheckOutDate { get; set; }
        
        [Required(ErrorMessage = "Number of guests is required")]
        [Display(Name = "Number of Adults")]
        [Range(1, 20, ErrorMessage = "Number of guests must be between 1 and 20")]
        public int NumberOfGuests { get; set; }
        
        [Display(Name = "Number of Children")]
        [Range(0, 20, ErrorMessage = "Number of children must be between 0 and 20")]
        public int NumberOfChildrens { get; set; }
        
        [Required(ErrorMessage = "Total amount is required")]
        [Display(Name = "Total Amount")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0")]
        public decimal? TotalAmount { get; set; }
        
        [Display(Name = "Discount Amount")]
        [Range(0, double.MaxValue, ErrorMessage = "Discount amount cannot be negative")]
        public decimal? DiscountAmount { get; set; }
        
        [Required(ErrorMessage = "Booking status is required")]
        [Display(Name = "Booking Status")]
        public string? BookingStatus { get; set; }

        // Navigation properties for display
        public string? CustomerName { get; set; }
        public string? RoomNumber { get; set; }
        public decimal? RoomPrice { get; set; }
    }
}