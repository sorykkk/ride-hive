using System.ComponentModel;

namespace RideHiveApi.Models.DataTransferObjects
{
    public class CarResponseDto
    {
        public int CarId { get; set; }
        public string OwnerId { get; set; } = string.Empty;
        
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
        public string Fuel { get; set; } = string.Empty;               // "Petrol", "Diesel", etc. (enum values)
        public float? Consumption { get; set; }
        public string DriveDisplay { get; set; } = string.Empty;       // "Front-Wheel", "Rear-Wheel", etc.
        public string Drive { get; set; } = string.Empty;              // "FWD", "RWD", etc. (enum values)
        public string TransmissionDisplay { get; set; } = string.Empty; // "Manual", "Automatic", etc.
        public string Transmission { get; set; } = string.Empty;       // "Manual", "Automatic", etc. (enum values)
        public string BodyDisplay { get; set; } = string.Empty;        // "Sedan", "SUV", etc.
        public string Body { get; set; } = string.Empty;               // "Sedan", "SUV", etc. (enum values)
        public float Displacement { get; set; }
        public int HorsePower { get; set; }
        
        // Important details
        public string ConditionDisplay { get; set; } = string.Empty;   // "Excellent", "Good", etc.
        public string Condition { get; set; } = string.Empty;          // "BrandNew", "Used", etc. (enum values)
        public string VinNumber { get; set; } = string.Empty;
        
        // Car images
        public List<CarImageData> CarImages { get; set; } = new List<CarImageData>();

        // Helper method to get enum descriptions
        private static string GetEnumDescription(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo != null)
            {
                var descAttribute = (DescriptionAttribute?)Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute));
                if (descAttribute != null && !string.IsNullOrEmpty(descAttribute.Description))
                {
                    return descAttribute.Description;
                }
            }
            return value.ToString();
        }

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
                // Display values (for user-friendly display)
                FuelDisplay = GetEnumDescription(car.Fuel),
                Fuel = car.Fuel.ToString(),
                Consumption = car.Consumption,
                DriveDisplay = GetEnumDescription(car.Drive),
                Drive = car.Drive.ToString(),
                TransmissionDisplay = GetEnumDescription(car.Transmission),
                Transmission = car.Transmission.ToString(),
                BodyDisplay = GetEnumDescription(car.Body),
                Body = car.Body.ToString(),
                Displacement = car.Displacement,
                HorsePower = car.HorsePower,
                ConditionDisplay = GetEnumDescription(car.Condition),
                Condition = car.Condition.ToString(),
                VinNumber = car.VinNumber,
                CarImages = car.CarImages?.ToList() ?? new List<CarImageData>()
            };
        }
    }
}