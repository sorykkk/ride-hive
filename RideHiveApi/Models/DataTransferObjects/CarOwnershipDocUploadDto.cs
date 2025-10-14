using System.ComponentModel.DataAnnotations;

namespace RideHiveApi.Models.DataTransferObjects
{
    public class OwnershipDocumentUploadDto
    {
        [Required]
        public int CarId { get; set; }
        
        [Required]
        public IFormFile Document { get; set; } = null!;
    }
}