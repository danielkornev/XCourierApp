using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xamarin.Forms;

namespace XCourierApp.Droid.Storage.Serialization.Converters
{
    public class ColorConverter : JsonConverter<Color>
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var colorText = reader.GetString();

            return Color.FromHex(colorText);
        }

        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));

            var hex = value.ToHex();

            writer.WriteStringValue(hex);
        }
    } // class
} // namespace