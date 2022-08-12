using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;
using GeneralUnifiedTestSystemYard.Core.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace GeneralUnifiedTestSystemYard.Core.EntryPoints.SOCK;

public class GutsyServer : IGutsyEntryPoint
{
    /// <summary>
    ///     Sets state of the JSON converter, to be transferable.
    ///     Also to enforce Naming Strategy to Camel Case and enums to strings.
    /// </summary>
    private JsonSerializerSettings JsonSerializerSettings { get; } = new()
    {
        Formatting = Formatting.None,
        Converters =
        {
            new ComplexJsonConverter(),
            new StringEnumConverter()
        },
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy
            {
                ProcessDictionaryKeys = false
            }
        }
    };
    
    /// <summary>
    ///     Default port number is 57526. The digits on top of GUTSY letters on QWERTY keyboard.
    /// </summary>
    internal const int DefaultPort = 57526;
    internal const string DefaultHost = "127.0.0.1";

    public string Identifier => "SOCK";
    private GutsyCore _gutsy = null!;
    
    // Thread signal.
    private ManualResetEvent AllDone { get; } = new(false);

    /// <exception cref="SocketException"></exception>
    /// <exception cref="SecurityException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="AbandonedMutexException"></exception>
    /// <exception cref="FormatException"></exception>
    public void Start(GutsyCore gutsy, JToken? token)
    {
        _gutsy = gutsy;
        var settings = token?.ToObject<SocketSettings>();
        var port = settings?.Port ?? DefaultPort;
        var ip = IPAddress.Parse(settings?.Address??DefaultHost);
        
        //var ipAddress = ip ?? Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(IPAddress.Parse(DefaultHost));

        // Create a TCP/IP socket.
        using var listener = new Socket(
            ip.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);

        // Bind the socket to the local endpoint and listen for incoming connections.
        listener.Bind(new IPEndPoint(ip, port));
        listener.Listen(128);

        try
        {
            while (listener.IsBound)
            {
                // Set the event to non-signaled state.  
                AllDone.Reset();

                // Start an asynchronous socket to listen for connections.  
                listener.BeginAccept(AcceptCallback, listener);

                // Wait until a connection is made before continuing.  
                AllDone.WaitOne();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            _gutsy = null!;
        }
    }

    /// <exception cref="OverflowException"></exception>
    /// <exception cref="SocketException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    private void AcceptCallback(IAsyncResult result)
    {
        // Signal the main thread to continue.  
        AllDone.Set();

        // Get the socket that handles the client request.  
        if (result.AsyncState is Socket listener && listener.EndAccept(result) is var handler)
        {
            // Create the state object.
            var state = new GutsyClientState(handler);
            handler.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, ReadCallback, state);
        }
    }

    /// <exception cref="OverflowException"></exception>
    /// <exception cref="SocketException"></exception>
    private void ReadCallback(IAsyncResult result)
    {
        // Retrieve the state object and the handler socket  
        // from the asynchronous state object.  
        if (result.AsyncState is not GutsyClientState state) return;
        
        var handler = state.WorkSocket;

        // Read data from the client socket.
        if (handler.EndReceive(result) is not (var bytesRead and > 0)) return;
        // There  might be more data, so store the data received so far.  
        state.StringBuilder.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));

        // Check for end-of-file tag. If it is not there, read
        // more data. 
        if (state.StringBuilder.ToString() is var content && content.IndexOf('\n') > -1)
        {
            // Send the data back to the client.
            var returnStr = Process(state.StringBuilder);
            state.StringBuilder.Clear();
            Send(handler, Encoding.ASCII.GetBytes(returnStr));
        }
        else
        {
            // Not all data received. Get more.  
            handler.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, ReadCallback, state);
        }
    }

    private string Process(StringBuilder data) => _gutsy.ProcessJson(data.ToString(),JsonSerializerSettings);

    /// <exception cref="SocketException"></exception>
    /// <exception cref="OverflowException"></exception>
    private static void Send(Socket handler, byte[] data) =>
        // Convert the string data to byte data using ASCII encoding.  
        //var byteData = Encoding.ASCII.GetBytes(data);
        // Begin sending the data to the remote device.  
        handler.BeginSend(data, 0, data.Length, 0, SendCallback, handler);

    /// <exception cref="IOException"></exception>
    private static void SendCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.  
            if (ar.AsyncState is Socket handler)
            {
                // Complete sending the data to the remote device.  
                var bytesSent = handler.EndSend(ar);
                Console.WriteLine($"Sent {bytesSent} bytes to client.");

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}