using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RideHiveApi.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty; // Who receives this notification

        [Required]
        public string Type { get; set; } = string.Empty; // "booking_request", "booking_accepted", "booking_rejected"

        [Required]
        [ForeignKey("Request")]
        public int RequestId { get; set; }

        [Required]
        [ForeignKey("Post")]
        public int PostId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        [Required]
        public bool IsRead { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public AppUser? User { get; set; }
        public Request? Request { get; set; }
        public PostItem? Post { get; set; }
    }
}
