using Newtonsoft.Json.Linq;

namespace GeneralUnifiedTestSystemYard.Core.EntryPoints;

public record GutsySettings(string EntryPoint, JToken? Settings);