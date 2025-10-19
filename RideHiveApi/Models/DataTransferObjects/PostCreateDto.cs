using System.ComponentModel.DataAnnotations;

namespace RideHiveApi.Models.DataTransferObjects
{
    public class PostCreateDto
    {
        [Required]
        public string OwnerId { get; set; } = string.Empty;

        [Required]
        public int CarId { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 10, ErrorMessage = "Title should be between 10 and 50 characters long")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Required]
        [Range(1.0, 1_000_000.0, ErrorMessage = "Price cannot be below 1 €/day and exceed 1 000 000 €/day")]
        public decimal Price { get; set; }

        [StringLength(250, ErrorMessage = "Special requirements cannot exceed 250 characters")]
        public string? SpecialRequirements { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        // don't know if it should be returned as string or DateTime
        public ICollection<string> AvailableTimeSlots { get; set; } = new List<string>();

        

    }
}