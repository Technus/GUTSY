using Newtonsoft.Json.Linq;

namespace GeneralUnifiedTestSystemYard.Core;

public interface IGUTSYCommand
{
    /// <summary>
    /// JSON in JSON out
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public JToken Execute(JToken parameters);

    /// <summary>
    /// Gets command unique identifier
    /// </summary>
    /// <returns></returns>
    public string GetID();
}
