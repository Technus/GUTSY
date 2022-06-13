using Newtonsoft.Json.Linq;

namespace GeneralUnifiedTestSystemYard.Core;

public class GUTSYResponse : GUTSYRequest
{
    public JToken? Result { get; set; }
    public Exception? Exception { get; set; }
}
