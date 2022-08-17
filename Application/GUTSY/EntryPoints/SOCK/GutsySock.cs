using System.Net;
using Newtonsoft.Json.Linq;

namespace GeneralUnifiedTestSystemYard.EntryPoints.SOCK;

public class GutsySock : IGutsyEntryPoint
{
    /// <summary>
    ///     Default port number is 57526. The digits on top of GUTSY letters on QWERTY keyboard.
    /// </summary>
    private const int DefaultPort = 57526;

    private const string DefaultHost = "127.0.0.1";
    public string Identifier => "SOCK";
    public void Start(GutsyCore gutsy, JToken? token = null)
    {
        var settings = token?.ToObject<SocketSettings>();
        var port = settings?.Port ?? DefaultPort;
        var ip = IPAddress.Parse(settings?.Address ?? DefaultHost);

        //var ipAddress = ip ?? Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(IPAddress.Parse(DefaultHost));

        new GutsyServer(gutsy, port, ip);
    }
}