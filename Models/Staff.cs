using System.ComponentModel.DataAnnotations;

namespace FrontendHotelManagementSystem.Models
{
    public class Staff
    {
        public int StaffId { get; set; }
        
        [Required(ErrorMessage = "Full name is required")]
        [Display(Name = "Full Name")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public string? FullName { get; set; }
        
        [Required(ErrorMessage = "Role is required")]
        public string? Role { get; set; }
        
        [Required(ErrorMessage = "Contact number is required")]
        [Display(Name = "Contact Number")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? ContactNumber { get; set; }
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }
        
        [Required(ErrorMessage = "Joining date is required")]
        [Display(Name = "Joining Date")]
        [DataType(DataType.Date)]
        public DateTime? JoiningDate { get; set; }
    }
}