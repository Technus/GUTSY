using Newtonsoft.Json.Linq;
using System.Net.Sockets;
using System.Text;

namespace GeneralUnifiedTestSystemYard.Core.Networking;

public class GUTSYClientState
{
    internal byte[] Buffer { get; }
    internal Socket WorkSocket { get; }
    internal StringBuilder StringBuilder { get; } = new StringBuilder();

    public JToken? state { get; set; }

    public GUTSYClientState(Socket workSocket, int bufferSize = 1024)
    {
        WorkSocket = workSocket;
        Buffer = new byte[bufferSize];
    }
}