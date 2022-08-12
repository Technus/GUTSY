namespace GeneralUnifiedTestSystemYard.Core.Module;

public interface IGutsyModule : IIdentifiable
{
    void Activate(GutsyCore gutsy);
}