using GeneralUnifiedTestSystemYard.Core;
using Ivi.Visa;

namespace GeneralUnifiedTestSystemYard.Commands.VISA;

public interface IVisaSessionResolver : IIdentifiable
{
    /// <summary>
    ///     Transforms resource from base resource manger into its own list of resources
    /// </summary>
    /// <param name="resourceManager"></param>
    /// <param name="resource"></param>
    /// <returns></returns>
    List<string> ResolveResources(IResourceManager resourceManager, string resource);

    /// <summary>
    ///     Called on own resource to create a session for it
    /// </summary>
    /// <param name="resourceManager"></param>
    /// <param name="resource"></param>
    /// <returns></returns>
    IVisaSession? ResolveSession(IResourceManager resourceManager, string resource);
}