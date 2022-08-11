using Newtonsoft.Json.Linq;

namespace GeneralUnifiedTestSystemYard.Core.Command;

public interface IGutsyCommand : IIdentifiable
{
    /// <summary>
    /// JSON in JSON out
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    JToken? Execute(JToken? parameters);
}
