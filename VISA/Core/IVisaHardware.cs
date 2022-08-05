using GeneralUnifiedTestSystemYard.Core;
using Ivi.Visa;

namespace GeneralUnifiedTestSystemYard.Commands.VISA;

public interface IVisaHardware : IIdentifiable
{
    private const string identifyQuery1 = "*IDN?";
    private const string identifyQuery2 = "ID?";//todo check
    public IVisaSession Session { get; }
    public bool Valid { get; }
}
