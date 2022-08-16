using System.Resources;
using GeneralUnifiedTestSystemYard.Core.ClassExtensions;

namespace CommandControl;

public class CommandControl
{
    public SortedDictionary<string, ICommandControlSessionResolver> SessionResolvers { get; } = new();
    public SortedDictionary<string, ICommandControlDeviceResolver> DeviceResolvers { get; } = new();
    
    public CommandControl()
    {
        SessionResolvers.LoadFromFolder("*GUTSY CommandControl Session Resolver*");
        DeviceResolvers.LoadFromFolder("*GUTSY CommandControl Device Resolver*");

        if (SessionResolvers.IsEmpty())
            throw new DllNotFoundException("Could not find any CommandControl session resolver");
    }
}