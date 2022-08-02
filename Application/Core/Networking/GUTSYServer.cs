using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Security;

namespace GeneralUnifiedTestSystemYard.Core.Networking;

/// <summary>
/// Default port number is 57526. The digits on top of GUTSY letters on QUERTY keyboard.
/// </summary>
public static class GUTSYServer
{
    private const int defaultPort = 57526;
    private const string defaultHost = "127.0.0.1";

    // Thread signal.  
    public static ManualResetEvent AllDone { get; } = new ManualResetEvent(false);

    /// <exception cref="FormatException"></exception>
    /// <exception cref="SocketException"></exception>
    /// <exception cref="SecurityException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="AbandonedMutexException"></exception>
    public static void StartListeningOnAddress(int port = defaultPort, string address = defaultHost) =>
        StartListening(port, IPAddress.Parse(address));

    /// <exception cref="FormatException"></exception>
    /// <exception cref="SocketException"></exception>
    /// <exception cref="SecurityException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="AbandonedMutexException"></exception>
    public static void StartListeningOnLocalHost(int port = defaultPort) =>
        StartListeningOnAddress(port, defaultHost);

    /// <exception cref="SocketException"></exception>
    /// <exception cref="SecurityException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="AbandonedMutexException"></exception>
    /// <exception cref="FormatException"></exception>
    public static void StartListening(int port = defaultPort, IPAddress? ip = null)
    {
        var ipAddress = ip ?? Dns.GetHostEntry(
            Dns.GetHostName()).AddressList.FirstOrDefault(IPAddress.Parse(defaultHost));

        // Create a TCP/IP socket.
        using var listener = new Socket(
            ipAddress.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);

        // Bind the socket to the local endpoint and listen for incoming connections.
        listener.Bind(new IPEndPoint(ipAddress, port));
        listener.Listen(128);

        while (true)
        {
            // Set the event to nonsignaled state.  
            AllDone.Reset();

            // Start an asynchronous socket to listen for connections.  
            listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

            // Wait until a connection is made before continuing.  
            AllDone.WaitOne();
        }
    }

    /// <exception cref="OverflowException"></exception>
    /// <exception cref="SocketException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public static void AcceptCallback(IAsyncResult ar)
    {
        // Signal the main thread to continue.  
        AllDone.Set();

        // Get the socket that handles the client request.  
        if (ar.AsyncState is Socket listener)
            if (listener.EndAccept(ar) is Socket handler)
            {
                // Create the state object.
                var state = new GUTSYClientState(handler);
                handler.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, new AsyncCallback(ReadCallback), state);
            }
    }

    /// <exception cref="OverflowException"></exception>
    /// <exception cref="SocketException"></exception>
    public static void ReadCallback(IAsyncResult ar)
    {
        // Retrieve the state object and the handler socket  
        // from the asynchronous state object.  
        if (ar.AsyncState is GUTSYClientState state)
        {
            Socket handler = state.WorkSocket;

            // Read data from the client socket.
            if (handler.EndReceive(ar) is int bytesRead && bytesRead>0)
            {
                // There  might be more data, so store the data received so far.  
                state.StringBuilder.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read
                // more data. 
                if (state.StringBuilder.ToString() is string content && content.IndexOf('\n') > -1)
                {
                    // Send the data back to the client.
                    var returnStr=Process(state.StringBuilder);
                    state.StringBuilder.Clear();
                    Send(handler, Encoding.ASCII.GetBytes(returnStr));
                }
                else
                {
                    // Not all data received. Get more.  
                    handler.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }
    }

    private static string Process(StringBuilder data) //TODO make async?  
    {
        return GUTSY.ProcessJSON(data.ToString());
    }

    /// <exception cref="SocketException"></exception>
    /// <exception cref="OverflowException"></exception>
    private static void Send(Socket handler, byte[] data)
    {
        // Convert the string data to byte data using ASCII encoding.  
        //var byteData = Encoding.ASCII.GetBytes(data);

        // Begin sending the data to the remote device.  
        handler.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallback), handler);
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
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

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