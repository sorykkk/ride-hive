using System.ComponentModel.DataAnnotations;

namespace RideHiveApi.Models.DataTransferObjects
{
    public class UserAuthResponseDto
    {
        public string UserId { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int? Phone { get; set; }
        public int? Age { get; set; }
        public string? Bio { get; set; }
        public string? Location { get; set; }
        public bool HasProfileImage { get; set; }
        public DateTime RegisteredAt { get; set; }
    }
}