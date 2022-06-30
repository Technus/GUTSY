using GeneralUnifiedTestSystemYard.Core;
using Ivi.Visa;

namespace GeneralUnifiedTestSystemYard.Commands.VISA;

public interface IVisaDeviceResolver : IIdentifiable
{
    bool CanConvert(VisaHardware hardware);
    IVisaDevice GetResourceManager(VisaHardware hardware);
}
