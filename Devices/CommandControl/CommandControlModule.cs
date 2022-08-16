using GeneralUnifiedTestSystemYard.Core;
using GeneralUnifiedTestSystemYard.Core.Module;

namespace CommandControl;

public class CommandControlModule : IGutsyModule
{
    private CommandControl CommandControl { get; set; } = null!;
    
    public string Identifier => "Command control";
    
    public void Activate(GutsyCore gutsy)
    {
        CommandControl = new CommandControl();
    }
}