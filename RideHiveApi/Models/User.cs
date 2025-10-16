using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RideHiveApi.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        public DateTime RegisteredAt { get; set; }
        public int? Age { get; set; }
        public int? Phone { get; set; }
        [MaxLength(5 * 1024 * 1024, ErrorMessage = "Image size cannot exceed 5MB")]
        public byte[]? ImageDrivingLicense { get; set; }
        
    }
}