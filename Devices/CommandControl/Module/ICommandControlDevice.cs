using GeneralUnifiedTestSystemYard;
using Newtonsoft.Json.Linq;

namespace CommandControl;

public interface ICommandControlDevice : IIdentifiable
{
    /**
     * The sender should always be the implementation of this interface
     */
    event EventHandler<JObject?> IncomingPacketEvent;

    void SendPacket(JObject? packet);
    
    ICommandControlSession Session { get; }
}