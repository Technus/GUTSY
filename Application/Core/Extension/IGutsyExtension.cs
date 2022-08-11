namespace GeneralUnifiedTestSystemYard.Core.Extension;

public interface IGutsyModule : IIdentifiable
{
    void Activate(GutsyCore gutsy);
}