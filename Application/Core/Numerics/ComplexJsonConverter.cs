using Newtonsoft.Json;
using System.Numerics;

namespace GeneralUnifiedTestSystemYard.Core.Numerics;

public class ComplexJsonConverter : JsonConverter<Complex>
{
    public override Complex ReadJson(JsonReader reader, Type objectType, Complex existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return reader.Value?.ToString()?.FromPolar() ?? Complex.NaN;
    }

    public override void WriteJson(JsonWriter writer, Complex value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToPolar(false));
    }
}
