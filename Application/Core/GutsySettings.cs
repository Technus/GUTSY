using GeneralUnifiedTestSystemYard.Core.Networking;

namespace GeneralUnifiedTestSystemYard.Core;

public class GutsySettings
{
    public GutsySettings(RunType runType = RunType.Server, int port = GutsyServer.DefaultPort,
        string address = GutsyServer.DefaultHost)
    {
        RunType = runType;
        Port = port;
        Address = address;
    }

    public RunType RunType { get; }
    public int? Port { get; }
    public string? Address { get; }
}

public enum RunType : byte
{
    Server,
    Console
}