using GeneralUnifiedTestSystemYard.Commands.VISA;
using Ivi.Visa;
using NationalInstruments.Visa;

namespace GeneralUnifiedTestSystemYard.Commands.NiVisa;

public class NiVisa : IVisaResourceManagerSupplier
{
    public string Identifier => "NI VISA.NET 21.5 v4.0.30319 + VISA.NET Shared Components 5.11.0 v2.0.50727";

    /// <exception cref="DllNotFoundException"></exception>
    public IResourceManager GetResourceManager()
    {
        try
        {
            return new ResourceManager();
        }
        catch (Exception e)
        {
            throw new DllNotFoundException("This VISA driver needs NI VISA.NET 21.5 installed", e);
        }
    }
}