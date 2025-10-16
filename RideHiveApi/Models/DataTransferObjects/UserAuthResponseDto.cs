using System.ComponentModel.DataAnnotations;

namespace RideHiveApi.Models.DataTransferObjects
{
    public class UserAuthResponseDto
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Role { get; set; }
        public DateTime RegisteredAt { get; set; }
    }
}