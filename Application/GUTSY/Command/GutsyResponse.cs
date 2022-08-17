using Newtonsoft.Json.Linq;

namespace GeneralUnifiedTestSystemYard.Command;

public class GutsyResponse : GutsyRequest
{
    public JToken? Result { get; set; }
    public Exception? Exception { get; set; }
}