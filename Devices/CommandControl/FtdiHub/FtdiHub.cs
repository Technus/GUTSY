using CommandControl;

namespace FtdiHub;

public class FtdiHub : ICommandControlSessionResolver
{
    public string Identifier => "FtdiHub";
    
    public bool CanResolve(string hardware)
    {
        throw new NotImplementedException();
    }

    public ICommandControlSession? ResolveDevice(string hardware)
    {
        throw new NotImplementedException();
    }
}