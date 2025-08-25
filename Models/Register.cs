using System.ComponentModel.DataAnnotations;
namespace HolidayHome.Models
{
   
    public class RegisterModel
    {
        [Required]
        [StringLength(20, ErrorMessage = "First name cannot exceed 20 characters.")]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Last name cannot exceed 20 characters.")]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [RegularExpression(@"^\(?([2-9][0-9]{2})\)?[-.●\s]?([0-9]{3})[-.●\s]?([0-9]{4})$",
            ErrorMessage = "Phone number must be a valid US number.")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Address Line 1")]
        public string? AddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string? AddressLine2 { get; set; }

        [Required]
        [RegularExpression("^[A-Z]{2}$", ErrorMessage = "Use 2-letter state abbreviation (e.g., NY, CA).")]
        public string? State { get; set; }

        [Required]
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Zipcode must be 5 or 9 digit US ZIP.")]
        public string? ZipCode { get; set; }
    }

}
