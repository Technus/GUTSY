using GeneralUnifiedTestSystemYard.Core;
using GeneralUnifiedTestSystemYard.Core.Module;

namespace SpaceNavigator;

internal class Navigation3DModule : IGutsyModule
{
    public Navigation3D? Navigation3D { get; private set; }

    public void Activate(GutsyCore gutsy)
    {
        Navigation3D ??= new Navigation3D();
    }

    public string Identifier => "Navigation3D";
}