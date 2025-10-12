using System.Text.Json;
using System.Text.Json.Serialization;

namespace RideHiveApi.Models.Converters
{
    public class EnumDescriptionConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsEnum;
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            // Create converter for the specific enum type
            var converterType = typeof(EnumDescriptionConverter<>).MakeGenericType(typeToConvert);
            return (JsonConverter?)Activator.CreateInstance(converterType);
        }
    }
}
