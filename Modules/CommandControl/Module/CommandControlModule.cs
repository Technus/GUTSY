using GeneralUnifiedTestSystemYard;
using GeneralUnifiedTestSystemYard.Module;

namespace CommandControl;

public class CommandControlModule : IGutsyModule
{
    private CommandControl CommandControl { get; set; } = null!;
    
    public string Identifier => "Command control";
    
    public void Activate(Gutsy gutsy)
    {
        CommandControl = new CommandControl();
    }
}