using System.ComponentModel.DataAnnotations;

namespace RideHiveApi.Models.DataTransferObjects
{
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password should be at least from 8 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [RegularExpression("^(Client|Owner)$", ErrorMessage = "Role must be either 'Client' or 'Owner'")]
        public string Role { get; set; }

        public int? Age { get; set; }
        
        public int? Phone { get; set; }

        public IFormFile? DrivingLicenseImage { get; set; }
    }
}