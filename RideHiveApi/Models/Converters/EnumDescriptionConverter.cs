using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RideHiveApi.Models.Converters
{
    public class EnumDescriptionConverter<T> : JsonConverter<T> where T : Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? str = reader.GetString();
            
            if (string.IsNullOrEmpty(str))
                throw new JsonException($"Cannot convert null or empty string to {typeof(T).Name}");

            // First try to match by description
            foreach (var field in typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr)
                {
                    if (attr.Description.Equals(str, StringComparison.OrdinalIgnoreCase))
                        return (T)field.GetValue(null)!;
                }
            }

            // If no description match, try enum name
            if (Enum.TryParse(typeof(T), str, true, out var result))
                return (T)result;

            throw new JsonException($"Unable to convert \"{str}\" to {typeof(T).Name}");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var field = typeof(T).GetField(value.ToString()!);
            if (field != null && Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr)
            {
                writer.WriteStringValue(attr.Description);
            }
            else
            {
                writer.WriteStringValue(value.ToString());
            }
        }
    }
}
