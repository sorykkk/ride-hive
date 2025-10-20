using System.ComponentModel.DataAnnotations;

namespace RideHiveApi.Models
{
    public class PostItem
    {
        [Key]
        public int PostId { get; set; }

        [Required]
        public string OwnerId { get; set; } = string.Empty;

        // Navigation property
        public Owner? Owner { get; set; }

        [Required]
        public int CarId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [MaxLength(250)]
        public string? SpecialRequirements { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public ICollection<DateTime> AvailableTimeSlots { get; set; } = new List<DateTime>();

        [Required]
        public DateTime PostedAt { get; set; } = DateTime.UtcNow;

        public bool Available { get; set; }
    }
}