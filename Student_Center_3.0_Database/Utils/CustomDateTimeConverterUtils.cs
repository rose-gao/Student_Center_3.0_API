using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Student_Center_3._0_Database.Utils
{
    public class CustomDateTimeConverterUtils : JsonConverter<DateTime>
    {
        private const string Format = "yyyy-MM-dd"; // Note: MM for month, not mm

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
        }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (DateTime.TryParseExact(reader.GetString(), Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return date;
            }

            throw new JsonException($"Invalid date format. Expected format: {Format}");
        }
    }
}
