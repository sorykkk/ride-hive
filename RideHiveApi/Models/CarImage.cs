using System.ComponentModel.DataAnnotations;

namespace RideHiveApi.Models
{
    public class CarImageData
    {
        [Key]
        public int CarImageId { get; set; }
        
        public int CarId { get; set; }

        [MaxLength(500)]
        public string ImagePath { get; set; } = string.Empty;

        [MaxLength(50)]
        public string ImageContentType { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}