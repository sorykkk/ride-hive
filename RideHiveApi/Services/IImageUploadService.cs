namespace RideHiveApi.Services
{
    public interface IImageUploadService
    {
        Task<string> SaveImageAsync(IFormFile imageFile, string folder = "car-images");
        Task<string> SaveDocumentAsync(IFormFile documentFile, string folder = "ownership-documents");
        Task<bool> DeleteImageAsync(string imagePath);
        Task<bool> DeleteDocumentAsync(string documentPath);
        bool IsValidImageFile(IFormFile file);
        bool IsValidDocumentFile(IFormFile file);
        string GetImageUrl(string imagePath);
        string GetDocumentUrl(string documentPath);
    }
}