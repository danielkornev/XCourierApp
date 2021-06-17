using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace XCourierApp.Storage.Serialization.Converters
{
    public class SKRectConverter : JsonConverter<SKRect>
    {
        public override SKRect Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                var text = reader.GetString();
                var parts = (from item in text.Split(new[] { ',' })
                             select float.Parse(item, CultureInfo.InvariantCulture)).ToArray();

                if (parts.Length != 4) return SKRect.Empty;

                return new SKRect(parts[0], parts[1], parts[2], parts[3]);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Error parsing Rect:{e.Message}");

                // Crashes.TrackError(e);
            }

            return SKRect.Empty;
        }

        public override void Write(Utf8JsonWriter writer, SKRect value, JsonSerializerOptions options)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));

            writer.WriteStringValue($"{value.Left},{value.Top},{value.Right},{value.Bottom}");
        }
    } // class
} // namespace
