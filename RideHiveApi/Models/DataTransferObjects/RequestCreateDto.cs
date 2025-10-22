using System.ComponentModel.DataAnnotations;

namespace RideHiveApi.Models.DataTransferObjects
{
    public class RequestCreateDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int PostId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one date must be selected")]
        public List<DateTime> RequestedDates { get; set; } = new List<DateTime>();
    }
}
