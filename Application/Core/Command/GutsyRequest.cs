﻿using Newtonsoft.Json.Linq;

namespace GeneralUnifiedTestSystemYard.Core.Command;

public class GutsyRequest
{
    public Guid? Guid { get; set; }
    public string? Command { get; set; }
    public JToken? Parameters { get; set; }
}