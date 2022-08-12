using System.Text;
using Newtonsoft.Json.Linq;

namespace GeneralUnifiedTestSystemYard.Core.EntryPoints.SOCK;

public class GutsyClientState
{
    public GutsyClientState(System.Net.Sockets.Socket workSocket, int bufferSize = 1024)
    {
        WorkSocket = workSocket;
        Buffer = new byte[bufferSize];
    }

    internal byte[] Buffer { get; }
    internal System.Net.Sockets.Socket WorkSocket { get; }
    internal StringBuilder StringBuilder { get; } = new();

    public JToken? State { get; set; }
}