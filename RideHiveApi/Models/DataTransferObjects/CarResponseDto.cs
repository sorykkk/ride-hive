using RideHiveApi.Models.Extensions;

namespace RideHiveApi.Models.DataTransferObjects
{
    //todo: correct this class for better representation
    public class CarResponseDto
    {
        public int CarId { get; set; }
        public int OwnerId { get; set; }
        
        // Basic details
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string? Version { get; set; }
        public string Color { get; set; } = string.Empty;
        public int NumberDoors { get; set; }
        public int NumberSeats { get; set; }
        public int YearProduction { get; set; }
        public int Course { get; set; }
        
        // Specifications - these will show user-friendly descriptions
        public string FuelDisplay { get; set; } = string.Empty;        // "Gasoline", "Diesel", etc.
        public float? Consumption { get; set; }
        public string DriveDisplay { get; set; } = string.Empty;       // "Front-Wheel", "Rear-Wheel", etc.
        public string TransmissionDisplay { get; set; } = string.Empty; // "Manual", "Automatic", etc.
        public string BodyDisplay { get; set; } = string.Empty;        // "Sedan", "SUV", etc.
        public float Displacement { get; set; }
        public int HorsePower { get; set; }
        
        // Important details
        public string ConditionDisplay { get; set; } = string.Empty;   // "Excellent", "Good", etc.
        public string VinNumber { get; set; } = string.Empty;
        public string? Description { get; set; }
        
        // Images count or URLs
        //todo: make it get from table
        // public int ImageCount { get; set; }
        // public List<string>? ImageUrls { get; set; }

        // Static method to convert from CarItem to CarResponseDto
        public static CarResponseDto FromCarItem(CarItem car)
        {
            return new CarResponseDto
            {
                CarId = car.CarId,
                OwnerId = car.OwnerId,
                Brand = car.Brand,
                Model = car.Model,
                Version = car.Version,
                Color = car.Color,
                NumberDoors = car.NumberDoors,
                NumberSeats = car.NumberSeats,
                YearProduction = car.YearProduction,
                Course = car.Course,
                FuelDisplay = car.Fuel.GetDescription(),           // "Gasoline" instead of "Gasoline"
                Consumption = car.Consumption,
                DriveDisplay = car.Drive.GetDescription(),         // "Front-Wheel" instead of "FWD"
                TransmissionDisplay = car.Transmission.GetDescription(), // "Manual" instead of "Manual"
                BodyDisplay = car.Body.GetDescription(),           // "Sedan" instead of "Sedan"
                Displacement = car.Displacement,
                HorsePower = car.HorsePower,
                ConditionDisplay = car.Condition.GetDescription(), // "Excellent" instead of "Excellent"
                VinNumber = car.VinNumber
            };
        }
    }
}