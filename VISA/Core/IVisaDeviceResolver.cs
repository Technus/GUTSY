using GeneralUnifiedTestSystemYard.Core;

namespace GeneralUnifiedTestSystemYard.Commands.VISA;

public interface IVisaDeviceResolver : IIdentifiable
{
    bool CanConvert(IVisaHardware hardware);

    IVisaDevice? ResolveDevice(IVisaHardware hardware);
}
