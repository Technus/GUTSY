using Newtonsoft.Json.Linq;

namespace GeneralUnifiedTestSystemYard.Core;

public interface IGUTSYCommand : IIdentifiable
{
    /// <summary>
    /// JSON in JSON out
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public JToken? Execute(JToken? parameters);
}
