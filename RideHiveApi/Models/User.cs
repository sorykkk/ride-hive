using System.ComponentModel.DataAnnotations;

namespace RideHiveApi.Models
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [MinLength(8)]
        public string Password { get; set; }
        public DateTime RegisteredAt { get; set; }
        public int? Age { get; set; }
        public int? Phone { get; set; }

        [Required]
        [MaxLength(5 * 1024 * 1024, ErrorMessage = "Image size cannot exceed 5MB")]
        public byte[] ImageDrivingLicense { get; set; } = Array.Empty<byte>();
        
    }
}