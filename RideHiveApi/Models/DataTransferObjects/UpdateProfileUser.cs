using System.ComponentModel.DataAnnotations;

namespace RideHiveApi.Models.DataTransferObjects
{
    public class UpdateProfileDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Surname { get; set; } = string.Empty;

        public int? Phone { get; set; }

        public int? Age { get; set; }

        [MaxLength(500)]
        public string? Bio { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }
    }
}