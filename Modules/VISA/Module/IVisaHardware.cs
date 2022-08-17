using Ivi.Visa;

namespace GeneralUnifiedTestSystemYard.Commands.VISA;

public interface IVisaHardware : IIdentifiable
{
    private const string IdentifyQuery1 = "*IDN?";
    private const string IdentifyQuery2 = "ID?";
    public IVisaSession Session { get; }
    public bool Valid { get; }
}