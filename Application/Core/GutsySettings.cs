using GeneralUnifiedTestSystemYard.Core.Networking;

namespace GeneralUnifiedTestSystemYard.Core;

public class GutsySettings
{
    public RunType RunType { get; }
    public int? Port { get; }
    public string? Address { get; }

    public GutsySettings(RunType runType = RunType.Server, int port = GutsyServer.DefaultPort, string address = GutsyServer.DefaultHost)
    {
        RunType = runType;
        Port = port;
        Address = address;
    }
}

public enum RunType : byte
{
    Server,
    Console
}
