using System.ComponentModel;

namespace RideHiveApi.Models.DataTransferObjects
{
    public class PostResponseDto
    {
        public int PostId { get; set; }
        public string OwnerId { get; set; } = string.Empty;
        public int CarId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? SpecialRequirements { get; set; }
        public string Location { get; set; } = string.Empty;
        public List<string> AvailableTimeSlots { get; set; } = new List<string>();
        public string PostedAt { get; set; } = string.Empty;

        public bool Available { get; set; }

        public static PostResponseDto FromPostItem(PostItem post)
        {
            return new PostResponseDto
            {
                PostId = post.PostId,
                OwnerId = post.OwnerId,
                CarId = post.CarId,
                Title = post.Title,
                Description = post.Description,
                Price = post.Price,
                SpecialRequirements = post.SpecialRequirements,
                Location = post.Location,
                AvailableTimeSlots = post.AvailableTimeSlots
                    .Select(dt => dt.ToString("o")) // ISO 8601 format
                    .ToList(),
                PostedAt = post.PostedAt.ToString("o"),
                Available = post.Available
            };
        }

    }
}