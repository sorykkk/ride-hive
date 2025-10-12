using System.ComponentModel.DataAnnotations;

namespace RideHiveApi.Models.DataTransferObjects
{
    public class CarImageUploadDto
    {
        [Required]
        public int CarId { get; set; }

        [Required]
        public IFormFile Image { get; set; } = null!;
    }

}