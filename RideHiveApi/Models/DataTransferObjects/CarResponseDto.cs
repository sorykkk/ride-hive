using System;
using System.ComponentModel;
using System.Reflection;

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
        
        // Images count or URLs
        //todo: make it get from table
        // public int ImageCount { get; set; }
        // public List<string>? ImageUrls { get; set; }

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
                Consumption = car.Consumption,
                DriveDisplay = GetEnumDescription(car.Drive),
                TransmissionDisplay = GetEnumDescription(car.Transmission),
                BodyDisplay = GetEnumDescription(car.Body),
                Displacement = car.Displacement,
                HorsePower = car.HorsePower,
                ConditionDisplay = GetEnumDescription(car.Condition),
                VinNumber = car.VinNumber
            };
        }
    }
}