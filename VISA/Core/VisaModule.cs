using GeneralUnifiedTestSystemYard.Core;
using GeneralUnifiedTestSystemYard.Core.Extension;

namespace GeneralUnifiedTestSystemYard.Commands.VISA;

public class VisaModule : IGutsyModule
{
    public Visa? Visa { get; set; }

    /// <exception cref="DllNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="System.Security.SecurityException"></exception>
    public void Activate(GutsyCore gutsy) => Visa ??= new Visa();

    public string Identifier => "VISA";
}
