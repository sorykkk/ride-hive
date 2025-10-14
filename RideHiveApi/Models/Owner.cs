using System.ComponentModel.DataAnnotations;

namespace RideHiveApi.Models
{
    public class Owner{
        [Key]
        public int OwnerId { get; set; }
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
        
    }
}