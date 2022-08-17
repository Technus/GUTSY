namespace GeneralUnifiedTestSystemYard.Commands.VISA;

public interface IVisaDeviceResolver : IIdentifiable
{
    bool CanResolve(IVisaHardware hardware);

    IVisaDevice? ResolveDevice(IVisaHardware hardware);
}