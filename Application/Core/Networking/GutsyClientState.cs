using Newtonsoft.Json.Linq;
using System.Net.Sockets;
using System.Text;

namespace GeneralUnifiedTestSystemYard.Core.Networking;

public class GutsyClientState
{
    internal byte[] Buffer { get; }
    internal Socket WorkSocket { get; }
    internal StringBuilder StringBuilder { get; } = new();

    public JToken? State { get; set; }

    public GutsyClientState(Socket workSocket, int bufferSize = 1024)
    {
        WorkSocket = workSocket;
        Buffer = new byte[bufferSize];
    }
}