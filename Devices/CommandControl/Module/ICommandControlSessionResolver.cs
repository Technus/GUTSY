using GeneralUnifiedTestSystemYard;

namespace CommandControl;

public interface ICommandControlSessionResolver : IIdentifiable
{
    bool CanResolve(string hardware);

    ICommandControlSession? ResolveDevice(string hardware);
}