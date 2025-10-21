using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RideHiveApi.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Surname { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; }
        public int? Age { get; set; }
        public int? Phone { get; set; }
        [MaxLength(500)]
        public string? Bio { get; set; }
        [MaxLength(200)]
        public string? Location { get; set; }
        [MaxLength(5 * 1024 * 1024, ErrorMessage = "Image size cannot exceed 5MB")]
        public byte[]? ImageDrivingLicense { get; set; }
        [MaxLength(5 * 1024 * 1024, ErrorMessage = "Profile image size cannot exceed 5MB")]
        public byte[]? ProfileImage { get; set; }
        [MaxLength(100)]
        public string? ProfileImageContentType { get; set; }

    }
}