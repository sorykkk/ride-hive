using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Reflection;
using RideHiveApi.Models.Enums;

namespace RideHiveApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnumsController : ControllerBase
    {
        [HttpGet("fuel-types")]
        public ActionResult<IEnumerable<EnumOption>> GetFuelTypes()
        {
            return Ok(GetEnumOptions<FuelType>());
        }

        [HttpGet("drive-types")]
        public ActionResult<IEnumerable<EnumOption>> GetDriveTypes()
        {
            return Ok(GetEnumOptions<DriveTrainLayoutType>());
        }

        [HttpGet("transmission-types")]
        public ActionResult<IEnumerable<EnumOption>> GetTransmissionTypes()
        {
            return Ok(GetEnumOptions<TransmissionType>());
        }

        [HttpGet("body-types")]
        public ActionResult<IEnumerable<EnumOption>> GetBodyTypes()
        {
            return Ok(GetEnumOptions<BodyType>());
        }

        [HttpGet("condition-types")]
        public ActionResult<IEnumerable<EnumOption>> GetConditionTypes()
        {
            return Ok(GetEnumOptions<ConditionType>());
        }

        [HttpGet("all")]
        public ActionResult<EnumCollections> GetAllEnums()
        {
            return Ok(new EnumCollections
            {
                FuelTypes = GetEnumOptions<FuelType>(),
                DriveTypes = GetEnumOptions<DriveTrainLayoutType>(),
                TransmissionTypes = GetEnumOptions<TransmissionType>(),
                BodyTypes = GetEnumOptions<BodyType>(),
                ConditionTypes = GetEnumOptions<ConditionType>()
            });
        }

        private static IEnumerable<EnumOption> GetEnumOptions<T>() where T : struct, Enum
        {
            return Enum.GetValues<T>()
                .Select(enumValue =>
                {
                    var name = enumValue.ToString();
                    var field = typeof(T).GetField(name);
                    var description = field?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? name;
                    
                    return new EnumOption
                    {
                        Value = name,
                        Label = description,
                        NumericValue = Convert.ToInt32(enumValue)
                    };
                })
                .ToList();
        }
    }

    public class EnumOption
    {
        public string Value { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public int NumericValue { get; set; }
    }

    public class EnumCollections
    {
        public IEnumerable<EnumOption> FuelTypes { get; set; } = [];
        public IEnumerable<EnumOption> DriveTypes { get; set; } = [];
        public IEnumerable<EnumOption> TransmissionTypes { get; set; } = [];
        public IEnumerable<EnumOption> BodyTypes { get; set; } = [];
        public IEnumerable<EnumOption> ConditionTypes { get; set; } = [];
    }
}