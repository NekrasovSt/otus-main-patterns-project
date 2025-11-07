using System.Text.Json;
using System.Text.Json.Serialization;

namespace ServiceUtils.Serialization;

public class FlexibleObjectConverter : JsonConverter<object>
{
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                return reader.GetString();
                
            case JsonTokenType.Number:
                if (reader.TryGetInt32(out int intValue))
                    return intValue;
                if (reader.TryGetInt64(out long longValue))
                    return longValue;
                return reader.GetDouble();
                
            case JsonTokenType.True:
                return true;
                
            case JsonTokenType.False:
                return false;
                
            case JsonTokenType.StartArray:
                var list = new List<object?>();
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    list.Add(Read(ref reader, typeToConvert, options));
                }
                return list;
                
            case JsonTokenType.StartObject:
                using (var document = JsonDocument.ParseValue(ref reader))
                {
                    return document.RootElement.Clone();
                }
                
            case JsonTokenType.Null:
                return null;
                
            default:
                throw new JsonException($"Unsupported token type: {reader.TokenType}");
        }
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}