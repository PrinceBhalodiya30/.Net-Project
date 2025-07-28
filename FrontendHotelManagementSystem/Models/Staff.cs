using System;
using System.ComponentModel.DataAnnotations;

namespace FrontendHotelManagementSystem.Models
{
    public class Staff
    {
        public int StaffId { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; } = null!;

        public string? Role { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        public string? ContactNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string? Email { get; set; }

        [Display(Name = "Joining Date")]
        [DataType(DataType.Date)]
        public DateOnly? JoiningDate { get; set; }
    }
}
