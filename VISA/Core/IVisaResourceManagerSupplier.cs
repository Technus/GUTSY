using GeneralUnifiedTestSystemYard.Core;
using Ivi.Visa;

namespace GeneralUnifiedTestSystemYard.Commands.VISA;

public interface IVisaResourceManagerSupplier : IIdentifiable
{
    IResourceManager GetResourceManager();
}