using System.ComponentModel.DataAnnotations;

namespace FrontendHotelManagementSystem.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        
        [Required(ErrorMessage = "Booking is required")]
        [Display(Name = "Booking")]
        public int BookingId { get; set; }
        
        [Required(ErrorMessage = "Payment date is required")]
        [Display(Name = "Payment Date")]
        [DataType(DataType.Date)]
        public DateTime? PaymentDate { get; set; }
        
        [Required(ErrorMessage = "Amount paid is required")]
        [Display(Name = "Amount Paid")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount paid must be greater than 0")]
        public decimal? AmountPaid { get; set; }
        
        [Required(ErrorMessage = "Payment mode is required")]
        [Display(Name = "Payment Mode")]
        public string? PaymentMode { get; set; }
    }
}