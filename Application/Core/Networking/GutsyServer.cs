using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;
using GeneralUnifiedTestSystemYard.Core.Exceptions;

namespace GeneralUnifiedTestSystemYard.Core.Networking;

/// <summary>
///     Default port number is 57526. The digits on top of GUTSY letters on QWERTY keyboard.
/// </summary>
public class GutsyServer
{
    internal const int DefaultPort = 57526;
    internal const string DefaultHost = "127.0.0.1";
    private readonly GutsyCore _gutsy;

    /// <exception cref="IOException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="SecurityException"></exception>
    /// <exception cref="ExtensionException"></exception>
    public GutsyServer(GutsyCore? gutsy = null)
    {
        _gutsy = gutsy ?? new GutsyCore();
    }

    // Thread signal.
    public ManualResetEvent AllDone { get; } = new(false);

    /// <exception cref="FormatException"></exception>
    /// <exception cref="SocketException"></exception>
    /// <exception cref="SecurityException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="AbandonedMutexException"></exception>
    public void StartListeningOnAddress(int port = DefaultPort, string address = DefaultHost)
    {
        StartListening(port, IPAddress.Parse(address));
    }

    /// <exception cref="FormatException"></exception>
    /// <exception cref="SocketException"></exception>
    /// <exception cref="SecurityException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="AbandonedMutexException"></exception>
    public void StartListeningOnLocalHost(int port = DefaultPort)
    {
        StartListeningOnAddress(port);
    }

    /// <exception cref="SocketException"></exception>
    /// <exception cref="SecurityException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="AbandonedMutexException"></exception>
    /// <exception cref="FormatException"></exception>
    public void StartListening(int port = DefaultPort, IPAddress? ip = null)
    {
        var ipAddress = ip ?? Dns.GetHostEntry(
            Dns.GetHostName()).AddressList.FirstOrDefault(IPAddress.Parse(DefaultHost));

        // Create a TCP/IP socket.
        using var listener = new Socket(
            ipAddress.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);

        // Bind the socket to the local endpoint and listen for incoming connections.
        listener.Bind(new IPEndPoint(ipAddress, port));
        listener.Listen(128);

        try
        {
            while (true)
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
    }

    /// <exception cref="OverflowException"></exception>
    /// <exception cref="SocketException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public void AcceptCallback(IAsyncResult ar)
    {
        // Signal the main thread to continue.  
        AllDone.Set();

        // Get the socket that handles the client request.  
        if (ar.AsyncState is Socket listener)
            if (listener.EndAccept(ar) is var handler)
            {
                // Create the state object.
                var state = new GutsyClientState(handler);
                handler.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, ReadCallback, state);
            }
    }

    /// <exception cref="OverflowException"></exception>
    /// <exception cref="SocketException"></exception>
    public void ReadCallback(IAsyncResult ar)
    {
        // Retrieve the state object and the handler socket  
        // from the asynchronous state object.  
        if (ar.AsyncState is GutsyClientState state)
        {
            var handler = state.WorkSocket;

            // Read data from the client socket.
            if (handler.EndReceive(ar) is var bytesRead and > 0)
            {
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
        }
    }

    private string Process(StringBuilder data) //TODO make async?  
    {
        return _gutsy.ProcessJson(data.ToString());
    }

    /// <exception cref="SocketException"></exception>
    /// <exception cref="OverflowException"></exception>
    private static void Send(Socket handler, byte[] data)
    {
        // Convert the string data to byte data using ASCII encoding.  
        //var byteData = Encoding.ASCII.GetBytes(data);

        // Begin sending the data to the remote device.  
        handler.BeginSend(data, 0, data.Length, 0, SendCallback, handler);
    }

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