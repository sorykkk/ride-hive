using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RideHiveApi.Models.Enums;

namespace RideHiveApi.Models
{
    public class Request
    {
        [Key]
        public int ReqId { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [ForeignKey("Post")]
        public int PostId { get; set; }

        [Required]
        public ICollection<DateTime> RequestedDates { get; set; } = new List<DateTime>();

        [Required]
        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}