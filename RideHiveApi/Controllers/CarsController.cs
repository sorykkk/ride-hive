using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RideHiveApi.Models.DataTransferObjects;
using RideHiveApi.Data;
using RideHiveApi.Models;
using RideHiveApi.Models.Enums;

namespace RideHiveApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CarsController> _logger;

        public CarsController(AppDbContext context, ILogger<CarsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/cars
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarResponseDto>>> GetAllCars()
        {
            try
            {
                var cars = await _context.CarItems
                    .Include(c => c.CarImages)
                    .ToListAsync();
                
                var response = cars.Select(CarResponseDto.FromCarItem).ToList();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all cars");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/cars/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CarResponseDto>> GetCar(int id)
        {
            try
            {
                var car = await _context.CarItems
                    .Include(c => c.CarImages)
                    .FirstOrDefaultAsync(c => c.CarId == id);

                if (car == null)
                    return NotFound($"Car with ID {id} not found");

                var response = CarResponseDto.FromCarItem(car);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving car with ID: {CarId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/cars/owner/{ownerId}
        [HttpGet("owner/{ownerId}")]
        public async Task<ActionResult<IEnumerable<CarResponseDto>>> GetCarsByOwner(int ownerId)
        {
            try
            {
                var cars = await _context.CarItems
                    .Include(c => c.CarImages)
                    .Where(c => c.OwnerId == ownerId)
                    .ToListAsync();

                var response = cars.Select(CarResponseDto.FromCarItem).ToList();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cars for owner: {OwnerId}", ownerId);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/cars
        [HttpPost]
        public async Task<ActionResult<CarItem>> CreateCar([FromForm] CarCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Validate ownership document
                if (dto.OwnershipDocument == null || dto.OwnershipDocument.Length == 0)
                    return BadRequest("Ownership document is required");

                var allowedDocTypes = new[] { "application/pdf", "image/jpeg", "image/png" };
                if (!allowedDocTypes.Contains(dto.OwnershipDocument.ContentType))
                    return BadRequest("Only PDF, JPEG, or PNG documents are allowed for ownership proof");

                // Check if VIN already exists
                var existingCar = await _context.CarItems.FirstOrDefaultAsync(c => c.VinNumber == dto.VinNumber);
                if (existingCar != null)
                    return Conflict("A car with this VIN number already exists");

                // Create car entity
                var car = new CarItem
                {
                    OwnerId = dto.OwnerId,
                    Brand = dto.Brand,
                    Model = dto.Model,
                    Version = dto.Version,
                    Color = dto.Color,
                    NumberDoors = dto.NumberDoors,
                    NumberSeats = dto.NumberSeats,
                    YearProduction = dto.YearProduction,
                    Course = dto.Course,
                    Fuel = Enum.Parse<FuelType>(dto.Fuel),
                    Consumption = dto.Consumption,
                    Drive = Enum.Parse<DriveTrainLayoutType>(dto.Drive),
                    Transmission = Enum.Parse<TransmissionType>(dto.Transmission),
                    Body = Enum.Parse<BodyType>(dto.Body),
                    Displacement = dto.Displacement,
                    HorsePower = dto.HorsePower,
                    Condition = Enum.Parse<ConditionType>(dto.Condition),
                    VinNumber = dto.VinNumber
                };

                // Handle ownership document
                using var docMs = new MemoryStream();
                await dto.OwnershipDocument.CopyToAsync(docMs);
                car.OwnershipDocumentData = docMs.ToArray();
                car.OwnershipDocumentContentType = dto.OwnershipDocument.ContentType;

                // Add car to context
                _context.CarItems.Add(car);
                await _context.SaveChangesAsync();

                // Handle car images if provided
                if (dto.CarImages != null && dto.CarImages.Any())
                {
                    await AddCarImages(car.CarId, dto.CarImages);
                }

                // Reload car with images
                var createdCar = await _context.CarItems
                    .Include(c => c.CarImages)
                    .FirstOrDefaultAsync(c => c.CarId == car.CarId);

                return CreatedAtAction(nameof(GetCar), new { id = car.CarId }, createdCar);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating car");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/cars/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCar(int id, [FromBody] UpdateCarDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var car = await _context.CarItems.FindAsync(id);
                if (car == null)
                    return NotFound($"Car with ID {id} not found");

                // Update only provided fields
                if (!string.IsNullOrEmpty(dto.Brand)) car.Brand = dto.Brand;
                if (!string.IsNullOrEmpty(dto.Model)) car.Model = dto.Model;
                if (dto.Version != null) car.Version = dto.Version;
                if (!string.IsNullOrEmpty(dto.Color)) car.Color = dto.Color;
                if (dto.NumberDoors.HasValue) car.NumberDoors = dto.NumberDoors.Value;
                if (dto.NumberSeats.HasValue) car.NumberSeats = dto.NumberSeats.Value;
                if (dto.YearProduction.HasValue) car.YearProduction = dto.YearProduction.Value;
                if (dto.Course.HasValue) car.Course = dto.Course.Value;
                if (!string.IsNullOrEmpty(dto.Fuel)) car.Fuel = Enum.Parse<FuelType>(dto.Fuel);
                if (dto.Consumption.HasValue) car.Consumption = dto.Consumption.Value;
                if (!string.IsNullOrEmpty(dto.Drive)) car.Drive = Enum.Parse<DriveTrainLayoutType>(dto.Drive);
                if (!string.IsNullOrEmpty(dto.Transmission)) car.Transmission = Enum.Parse<TransmissionType>(dto.Transmission);
                if (!string.IsNullOrEmpty(dto.Body)) car.Body = Enum.Parse<BodyType>(dto.Body);
                if (dto.Displacement.HasValue) car.Displacement = dto.Displacement.Value;
                if (dto.HorsePower.HasValue) car.HorsePower = dto.HorsePower.Value;
                if (!string.IsNullOrEmpty(dto.Condition)) car.Condition = Enum.Parse<ConditionType>(dto.Condition);

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating car with ID: {CarId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/cars/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            try
            {
                var car = await _context.CarItems
                    .Include(c => c.CarImages)
                    .FirstOrDefaultAsync(c => c.CarId == id);

                if (car == null)
                    return NotFound($"Car with ID {id} not found");

                _context.CarItems.Remove(car);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting car with ID: {CarId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/cars/{id}/images
        [HttpPost("{id}/images")]
        public async Task<IActionResult> UploadCarImage(int id, [FromForm] CarImageUploadDto dto)
        {
            try
            {
                if (dto.Image == null || dto.Image.Length == 0)
                    return BadRequest("No image uploaded");

                var car = await _context.CarItems.FindAsync(id);
                if (car == null)
                    return NotFound($"Car with ID {id} not found");

                if (dto.CarId != id)
                    return BadRequest("Car ID mismatch");

                // Validate file type
                var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
                if (!allowedTypes.Contains(dto.Image.ContentType))
                    return BadRequest("Only JPEG, PNG, or WEBP images are allowed");

                // Validate file size (5MB max)
                if (dto.Image.Length > 5 * 1024 * 1024)
                    return BadRequest("Image size cannot exceed 5MB");

                var imageData = new CarImageData
                {
                    CarId = id,
                    ImageContentType = dto.Image.ContentType,
                    UploadedAt = DateTime.UtcNow
                };

                using var ms = new MemoryStream();
                await dto.Image.CopyToAsync(ms);
                imageData.ImageData = ms.ToArray();

                _context.CarImages.Add(imageData);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Image uploaded successfully", imageId = imageData.CarImageId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image for car: {CarId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/cars/{carId}/images/{imageId}
        [HttpGet("{carId}/images/{imageId}")]
        public async Task<IActionResult> GetCarImage(int carId, int imageId)
        {
            try
            {
                var image = await _context.CarImages
                    .FirstOrDefaultAsync(img => img.CarId == carId && img.CarImageId == imageId);

                if (image == null)
                    return NotFound("Image not found");

                return File(image.ImageData, image.ImageContentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving image: {ImageId} for car: {CarId}", imageId, carId);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/cars/{carId}/images/{imageId}
        [HttpDelete("{carId}/images/{imageId}")]
        public async Task<IActionResult> DeleteCarImage(int carId, int imageId)
        {
            try
            {
                var image = await _context.CarImages
                    .FirstOrDefaultAsync(img => img.CarId == carId && img.CarImageId == imageId);

                if (image == null)
                    return NotFound("Image not found");

                _context.CarImages.Remove(image);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image: {ImageId} for car: {CarId}", imageId, carId);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/cars/{id}/ownership-document
        [HttpPost("{id}/ownership-document")]
        public async Task<IActionResult> UploadOwnershipDocument(int id, [FromForm] OwnershipDocumentUploadDto dto)
        {
            try
            {
                if (dto.Document == null || dto.Document.Length == 0)
                    return BadRequest("No document uploaded");

                var car = await _context.CarItems.FindAsync(id);
                if (car == null)
                    return NotFound($"Car with ID {id} not found");

                if (dto.CarId != id)
                    return BadRequest("Car ID mismatch");

                // Validate file type
                var allowedTypes = new[] { "application/pdf", "image/jpeg", "image/png" };
                if (!allowedTypes.Contains(dto.Document.ContentType))
                    return BadRequest("Only PDF, JPEG, or PNG documents are allowed");

                // Validate file size (10MB max)
                if (dto.Document.Length > 10 * 1024 * 1024)
                    return BadRequest("Document size cannot exceed 10MB");

                using var ms = new MemoryStream();
                await dto.Document.CopyToAsync(ms);
                
                car.OwnershipDocumentData = ms.ToArray();
                car.OwnershipDocumentContentType = dto.Document.ContentType;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Ownership document uploaded successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading ownership document for car: {CarId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/cars/{id}/ownership-document
        [HttpGet("{id}/ownership-document")]
        public async Task<IActionResult> GetOwnershipDocument(int id)
        {
            try
            {
                var car = await _context.CarItems.FindAsync(id);
                if (car == null || car.OwnershipDocumentData == null)
                    return NotFound("Document not found");

                return File(car.OwnershipDocumentData, car.OwnershipDocumentContentType ?? "application/pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ownership document for car: {CarId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // Helper method to add multiple car images
        private async Task AddCarImages(int carId, List<IFormFile> images)
        {
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            
            foreach (var image in images)
            {
                if (image.Length == 0) continue;
                
                if (!allowedTypes.Contains(image.ContentType)) continue;
                
                if (image.Length > 5 * 1024 * 1024) continue; // Skip if too large

                var imageData = new CarImageData
                {
                    CarId = carId,
                    ImageContentType = image.ContentType,
                    UploadedAt = DateTime.UtcNow
                };

                using var ms = new MemoryStream();
                await image.CopyToAsync(ms);
                imageData.ImageData = ms.ToArray();

                _context.CarImages.Add(imageData);
            }

            await _context.SaveChangesAsync();
        }
    }
}