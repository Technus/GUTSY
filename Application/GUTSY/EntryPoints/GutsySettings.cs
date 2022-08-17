using Newtonsoft.Json.Linq;

namespace GeneralUnifiedTestSystemYard.EntryPoints;

public record GutsySettings(string EntryPoint, JToken? Settings);