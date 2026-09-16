using Shared;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DTOs
{
    public class SmartRowVersionConverter: JsonConverter<byte[]>
    {
        public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string text = reader.GetString();
            return Helper.ParseRowVersion(text); // Use your helper logic here!
        }

        public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(Convert.ToBase64String(value));
        }
    }
}
