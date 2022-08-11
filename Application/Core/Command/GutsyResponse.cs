using Newtonsoft.Json.Linq;

namespace GeneralUnifiedTestSystemYard.Core.Command;

public class GutsyResponse : GutsyRequest
{
    public JToken? Result { get; set; }
    public Exception? Exception { get; set; }
}
