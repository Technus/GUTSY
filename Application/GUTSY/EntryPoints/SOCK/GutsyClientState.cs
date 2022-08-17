using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json.Linq;

namespace GeneralUnifiedTestSystemYard.EntryPoints.SOCK;

public class GutsyClientState
{
    public GutsyClientState(Socket workSocket, int bufferSize = 1024)
    {
        WorkSocket = workSocket;
        Buffer = new byte[bufferSize];
    }

    internal byte[] Buffer { get; }
    internal Socket WorkSocket { get; }
    internal StringBuilder StringBuilder { get; } = new();

    public JToken? State { get; set; }
}