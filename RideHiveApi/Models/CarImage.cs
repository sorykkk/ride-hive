using System.ComponentModel.DataAnnotations;
using RideHiveApi.Models.Validation;
using RideHiveApi.Models.Enums;

namespace RideHiveApi.Models
{
    public class CarImageData
    {
        [Key]
        public int CarImageId { get; set; }
        
        public int CarId { get; set; }

        [MaxLength(5 * 1024 * 1024, ErrorMessage = "Image size cannot exceed 5MB")]
        public byte[] ImageData { get; set; } = Array.Empty<byte>();

        [MaxLength(50)]
        public string ImageContentType { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}