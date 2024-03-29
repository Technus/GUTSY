﻿using GeneralUnifiedTestSystemYard.Module;

namespace GeneralUnifiedTestSystemYard.Commands.VISA;

public class VisaModule : IGutsyModule
{
    private Visa Visa { get; set; } = null!;

    /// <exception cref="DllNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="System.Security.SecurityException"></exception>
    public void Activate(Gutsy gutsy)
    {
        Visa = new Visa();
    }

    public string Identifier => "VISA";
}