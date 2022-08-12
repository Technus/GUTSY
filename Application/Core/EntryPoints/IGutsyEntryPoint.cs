using Newtonsoft.Json.Linq;

namespace GeneralUnifiedTestSystemYard.Core.EntryPoints;

public interface IGutsyEntryPoint : IIdentifiable
{
    void Start(GutsyCore gutsy, JToken? token = null);
}