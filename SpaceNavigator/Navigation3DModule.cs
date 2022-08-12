﻿using GeneralUnifiedTestSystemYard.Core;
using GeneralUnifiedTestSystemYard.Core.Module;

namespace SpaceNavigator;

internal class Navigation3DModule : IGutsyModule
{
    private Navigation3D Navigation3D { get; set; }

    public void Activate(GutsyCore gutsy)
    {
        Navigation3D = new Navigation3D();
    }

    public string Identifier => "Navigation3D";
}