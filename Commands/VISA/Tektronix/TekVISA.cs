using GeneralUnifiedTestSystemYard.Commands.VISA;

namespace GeneralUnifiedTestSystemYard.Commands.Tektronix;

public class TekVISA : IVISAResourceManager
{
    public string GetID()
    {
        return "TekVISA";
    }
}
