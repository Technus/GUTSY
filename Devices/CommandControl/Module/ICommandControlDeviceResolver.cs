using GeneralUnifiedTestSystemYard;

namespace CommandControl;

public interface ICommandControlDeviceResolver : IIdentifiable
{
    bool CanResolve(ICommandControlSession hardware);

    ICommandControlDevice? ResolveDevice(ICommandControlSession hardware);
}