using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GeneralUnifiedTestSystemYard.Core;

public class GUTSYRequest
{
    public Guid? Guid { get; set; }
    public string? Command { get; set; }
    public JToken? Parameters { get ; set ; }
}
