using System.ComponentModel.DataAnnotations;

// This is a mock implementation
namespace RideHiveApi.Models
{
    public class Owner
    {
        [Key]
        public string OwnerId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public ICollection<PostItem> Posts { get; set; } = new List<PostItem>();
    }
}