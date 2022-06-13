using GeneralUnifiedTestSystemYard.Commands.VISA;
using Ivi.Visa;
using NationalInstruments.Visa;

namespace GeneralUnifiedTestSystemYard.Commands.NiVisa;

public class NiVisa : IVisaResourceManagerSupplier
{
    public string GetID()
    {
        return "NI VISA.NET 21.5 v4.0.30319 + VISA.NET Shared Components 5.11.0 v2.0.50727";
    }

    public IResourceManager GetResourceManager()
    {
        return new ResourceManager();
    }
}
