namespace RideHiveApi.Services
{
    public interface IImageUploadService
    {
        Task<string> SaveImageAsync(IFormFile imageFile, string folder = "car-images");
        Task<bool> DeleteImageAsync(string imagePath);
        bool IsValidImageFile(IFormFile file);
        string GetImageUrl(string imagePath);
    }
}