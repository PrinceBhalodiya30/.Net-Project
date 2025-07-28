using System.ComponentModel.DataAnnotations;

namespace FrontendHotelManagementSystem.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }

        [Required(ErrorMessage = "Booking ID is required")]
        public int BookingId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Payment Date")]
        public DateTime? PaymentDate { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive")]
        public decimal? AmountPaid { get; set; }

        [StringLength(50)]
        public string? PaymentMode { get; set; }
        public decimal Amount { get; internal set; }
    }
}
