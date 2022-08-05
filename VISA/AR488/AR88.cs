using Ivi.Visa;
using System.Management;
using System.Text;

namespace GeneralUnifiedTestSystemYard.Commands.VISA;

public class AR488 : IVisaSessionResolver
{
    private readonly Dictionary<string, AR488Interface> interfaces = new();
    private const string identifyQuery1 = "*IDN?\n";
    private const string identifyQuery2 = "ID?\n";//todo check
    private const string serialName = "Arduino";
    private const string serialVidPid = "VID_2341&PID_8037";

    public string GetID() => "AR488";

    public List<string> ResolveResources(IResourceManager resourceManager, string resource)
    {
        var output = new List<string>();
        try
        {
            using var session = resourceManager.Open(resource, AccessModes.None, 1000);
            if (session is ISerialSession serial)
            {
                if (serial.HardwareInterfaceName.Contains(serialName) ||
                    GetSerialPortData(serial.HardwareInterfaceNumber).Contains(serialVidPid))
                {
                    var timeout = serial.TimeoutMilliseconds;
                    serial.TimeoutMilliseconds = 100;

                    var baud = serial.BaudRate;
                    try
                    {
                        //serial.BaudRate = 57600;
                        //if (TryFindInterface(serial)) goto found;
                        //serial.BaudRate = 115200;
                        //if (TryFindInterface(serial)) goto found;
                        //if (TryFindInterface(serial)) goto found;
                        //if (TryFindInterface(serial)) goto found;
                        //if (TryFindInterface(serial)) goto found;
                        serial.BaudRate = 1000000;
                        if (TryFindInterface(serial)) goto found;
                        if (TryFindInterface(serial)) goto found;
                        if (TryFindInterface(serial)) goto found;
                        if (TryFindInterface(serial)) goto found;
                        if (TryFindInterface(serial)) goto found;
                        if (TryFindInterface(serial)) goto found;
                        if (TryFindInterface(serial)) goto found;
                        if (TryFindInterface(serial)) goto found;
                        goto notFound;

                    found: 
                        try
                        {
                            serial.RawIO.Write("++read\n");
                            serial.RawIO.Write("++read\n");
                            try
                            {
                                while (true)//untill timout
                                {
                                    serial.RawIO.Read(1);
                                }
                            }
                            catch { }
                        }
                        catch { }
                        output.Add("GPIB::INTFC::AR488::" + resource);

                        //Thread.Sleep(100);
                        //serial.RawIO.Write($"++ifc\n");
                        //serial.RawIO.Write($"++read_tmo_ms {333}\n");
                        //serial.RawIO.Write($"++eot_char {(byte)'\n'}\n");
                        //serial.RawIO.Write($"++eot_enable {1}\n");
                        serial.TimeoutMilliseconds = 2000;

                        serial.TerminationCharacterEnabled = true;
                        serial.FlowControl = SerialFlowControlModes.None;
                        serial.ReadTermination = SerialTerminationMethod.TerminationCharacter;
                        serial.SetBufferSize(IOBuffers.ReadWrite, 1024 * 1024);

                        foreach (var device in TryFindInstruments(serial))
                        {
                            output.Add("GPIB::" + (device < 10 ? "0" : "") + device + "::0::INSTR::AR488::" + resource);
                        }

                        return output;
                    }
                    finally
                    {
                        serial.Flush(IOBuffers.ReadWrite, true);
                        serial.TimeoutMilliseconds = timeout;
                        serial.BaudRate = baud;
                        serial.Flush(IOBuffers.ReadWrite, true);
                    }
                }
            }
        notFound:
            return output;
        }
        catch
        {
        }
        return output;
    }

    /// <exception cref="FormatException"></exception>
    /// <exception cref="OverflowException"></exception>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public IVisaSession? ResolveSession(IResourceManager resourceManager, string resource)
    {
        if (resource.Contains("AR488"))
        {
            if (resource.Contains("INTFC"))
            {
                if (interfaces.ContainsKey(resource))
                {
                    //interfaces[resource].Dispose();
                    //interfaces.Remove(resource);
                    return interfaces[resource];
                }

                var session = resourceManager.Open(resource.Replace("GPIB::INTFC::AR488::", ""));
                if (session is ISerialSession serial)
                {
                    var controller = new AR488Interface(serial);
                    interfaces.Add(resource, controller);
                    return controller;
                }
            }
            else if (resource.Contains("INSTR"))
            {
                var path = resource.Split(':');
                var parent = "GPIB::INTFC::AR488::";
                var found = false;
                for (int i = 0; i < path.Length; i++)
                {
                    if (path[i].Contains("AR488"))
                    {
                        i += 2;
                        found = true;
                    }
                    if (found)
                    {
                        parent += path[i];
                        parent += ":";
                    }
                }
                parent = parent.Remove(parent.Length - 1);
                if (interfaces.ContainsKey(parent))
                {
                    return new AR488Session(interfaces[parent], short.Parse(path[2]), short.Parse(path[4]));
                }
                else if(ResolveSession(resourceManager, parent) is AR488Interface ar488)
                {
                    return new AR488Session(ar488, short.Parse(path[2]), short.Parse(path[4]));
                }
                else
                {
                    throw new ArgumentException($"Cannot link AR488 reference to device: {resource}");
                }
            }

            throw new NotImplementedException($"Unknown type for AR488: {resource}");
        }
        return null;
    }
    private static string GetSerialPortData(int number)
    {
        if (OperatingSystem.IsWindows())
        {
            using (var searcher = new ManagementObjectSearcher(
                @"\\" + Environment.MachineName + @"\root\CIMV2",
                $"SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM{number}%'"))
            {
                var ports = searcher.Get().Cast<ManagementBaseObject>().ToList();
                var port = ports[0];

                return port["DeviceID"].ToString() ?? $"COM{number}";
            }
        }
        return $"COM{number}";
    }

    private bool TryFindInterface(ISerialSession serial)
    {
        try
        {
            Thread.Sleep(50);
            serial.RawIO.Write("++ver\n");
            var ver = serial.RawIO.ReadString();
            return ver.StartsWith("AR488");
        }
        catch (Exception)
        {
            return false;
        }
    }

    private bool TryFindInstrument(ISerialSession serial, int addr)
    {
        //Thread.Sleep(100);
        serial.RawIO.Write($"++check {addr}\n");
        try
        {
            return serial.RawIO.ReadString().Contains("0");
        }
        catch (Exception e)
        {
            //Thread.Sleep(100);
            serial.Flush(IOBuffers.ReadWrite, true);
            serial.RawIO.Write($"++check {addr}\n");
            return serial.RawIO.ReadString().Contains("0");
        }
    }

    private byte[] TryFindInstruments(ISerialSession serial)
    {
        uint devices;
        //Thread.Sleep(100);
        serial.RawIO.Write($"++check all\n");
        try
        {
            devices = uint.Parse(serial.RawIO.ReadString());
        }
        catch (Exception e)
        {
            //Thread.Sleep(100);
            serial.Flush(IOBuffers.ReadWrite, true);
            serial.RawIO.Write($"++check all\n");
            devices = uint.Parse(serial.RawIO.ReadString());
        }
        List<byte> addresses = new List<byte>();
        for (byte device = 0; device < 30; device++)
        {
            if ((devices & (1 << device)) != 0)
            {
                addresses.Add(device);
            }
        }
        return addresses.ToArray();
    }

    private bool TryIdentify(ISerialSession serial)
    {
        string id;
        try
        {
            serial.RawIO.Write(identifyQuery1);
            Thread.Sleep(100);
            serial.RawIO.Write($"++read\n");
            id = serial.RawIO.ReadString();
            return id.Length > 0;
        }
        catch (Exception)
        {
            try
            {
                Thread.Sleep(100);
                serial.RawIO.Write(identifyQuery2);
                Thread.Sleep(100);
                serial.RawIO.Write($"++read\n");
                id = serial.RawIO.ReadString();
                return id.Length > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

internal class AR488Result : IVisaAsyncResult
{
    public bool IsAborted { get; private set; }

    public byte[] Buffer { get; private set; }

    public long Count { get; private set; }

    public long Index { get; private set; }

    public bool IsCompleted { get; private set; }

    public WaitHandle AsyncWaitHandle { get; private set; }

    public object AsyncState { get; private set; }

    public bool CompletedSynchronously { get; private set; }

    public AR488Result(
        bool isAborted,
        byte[] buffer,
        long count,
        long index,
        bool isCompleted,
        WaitHandle asyncWaitHandle,
        object asyncState,
        bool completedSynchronously)
    {
        IsAborted = isAborted;
        Buffer = buffer;
        Count = count;
        Index = index;
        IsCompleted = isCompleted;
        AsyncWaitHandle = asyncWaitHandle;
        AsyncState = asyncState;
        CompletedSynchronously = completedSynchronously;
    }
}

internal class AR488Raw : IMessageBasedRawIO, IVisaSession, IDisposable
{
    private IVisaSession _session;
    private ISerialSession _serial;
    private AR488Interface _interface;

    public AR488Raw(IVisaSession session, ISerialSession serial)
    {
        _session = session;
        _serial = serial;
        if (session is AR488Interface)
        {
            _interface = session as AR488Interface;
        }
        else if (session is AR488Session)
        {
            _interface = (session as AR488Session).Interface;
        }
    }

    public int EventQueueCapacity { get => _session.EventQueueCapacity; set => _session.EventQueueCapacity = value; }

    public string HardwareInterfaceName => _session.HardwareInterfaceName;
    public short HardwareInterfaceNumber => _session.HardwareInterfaceNumber;
    public HardwareInterfaceType HardwareInterfaceType => _session.HardwareInterfaceType;
    public string ResourceClass => _session.ResourceClass;
    public Version ResourceImplementationVersion => _session.ResourceImplementationVersion;
    public ResourceLockState ResourceLockState => _session.ResourceLockState;
    public short ResourceManufacturerId => _session.ResourceManufacturerId;
    public string ResourceManufacturerName => _session.ResourceManufacturerName;
    public string ResourceName => _session.ResourceName;
    public Version ResourceSpecificationVersion => _session.ResourceSpecificationVersion;

    public bool SynchronizeCallbacks { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int TimeoutMilliseconds { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void AbortAsyncOperation(IVisaAsyncResult result)
    {
        _serial.RawIO.AbortAsyncOperation(result);
    }

    public IVisaAsyncResult BeginRead(int count)
    {
        throw new NotImplementedException();
    }

    public IVisaAsyncResult BeginRead(int count, object state)
    {
        throw new NotImplementedException();
    }

    public IVisaAsyncResult BeginRead(int count, VisaAsyncCallback callback, object state)
    {
        throw new NotImplementedException();
    }

    public IVisaAsyncResult BeginRead(byte[] buffer)
    {
        throw new NotImplementedException();
    }

    public IVisaAsyncResult BeginRead(byte[] buffer, object state)
    {
        throw new NotImplementedException();
    }

    public IVisaAsyncResult BeginRead(byte[] buffer, long index, long count)
    {
        throw new NotImplementedException();
    }

    public IVisaAsyncResult BeginRead(byte[] buffer, long index, long count, object state)
    {
        throw new NotImplementedException();
    }

    public IVisaAsyncResult BeginRead(byte[] buffer, VisaAsyncCallback callback, object state)
    {
        throw new NotImplementedException();
    }

    public IVisaAsyncResult BeginRead(byte[] buffer, long index, long count, VisaAsyncCallback callback, object state)
    {
        throw new NotImplementedException();
    }

    public IVisaAsyncResult BeginWrite(string buffer)
    {
        throw new NotImplementedException();
    }

    public IVisaAsyncResult BeginWrite(string buffer, object state)
    {
        throw new NotImplementedException();
    }

    public IVisaAsyncResult BeginWrite(string buffer, VisaAsyncCallback callback, object state)
    {
        throw new NotImplementedException();
    }

    public IVisaAsyncResult BeginWrite(byte[] buffer)
    {
        throw new NotImplementedException();
    }

    public IVisaAsyncResult BeginWrite(byte[] buffer, object state)
    {
        throw new NotImplementedException();
    }

    public IVisaAsyncResult BeginWrite(byte[] buffer, long index, long count)
    {
        throw new NotImplementedException();
    }

    public IVisaAsyncResult BeginWrite(byte[] buffer, long index, long count, object state)
    {
        throw new NotImplementedException();
    }

    public IVisaAsyncResult BeginWrite(byte[] buffer, VisaAsyncCallback callback, object state)
    {
        throw new NotImplementedException();
    }

    public IVisaAsyncResult BeginWrite(byte[] buffer, long index, long count, VisaAsyncCallback callback, object state)
    {
        throw new NotImplementedException();
    }

    public long EndRead(IVisaAsyncResult result)
    {
        throw new NotImplementedException();
    }

    public string EndReadString(IVisaAsyncResult result)
    {
        throw new NotImplementedException();
    }

    public long EndWrite(IVisaAsyncResult result)
    {
        throw new NotImplementedException();
    }


    public void DisableEvent(EventType eventType)
    {
        throw new NotImplementedException();
    }

    public void DiscardEvents(EventType eventType)
    {
        throw new NotImplementedException();
    }

    public void EnableEvent(EventType eventType)
    {
        throw new NotImplementedException();
    }

    public VisaEventArgs WaitOnEvent(EventType eventType)
    {
        throw new NotImplementedException();
    }

    public VisaEventArgs WaitOnEvent(EventType eventType, out EventQueueStatus status)
    {
        throw new NotImplementedException();
    }

    public VisaEventArgs WaitOnEvent(EventType eventType, int timeoutMilliseconds)
    {
        throw new NotImplementedException();
    }

    public VisaEventArgs WaitOnEvent(EventType eventType, TimeSpan timeout)
    {
        throw new NotImplementedException();
    }

    public VisaEventArgs WaitOnEvent(EventType eventType, int timeoutMilliseconds, out EventQueueStatus status)
    {
        throw new NotImplementedException();
    }

    public VisaEventArgs WaitOnEvent(EventType eventType, TimeSpan timeout, out EventQueueStatus status)
    {
        throw new NotImplementedException();
    }

    public void LockResource()
    {
        _serial.LockResource();
    }

    public void LockResource(TimeSpan timeout)
    {
        _serial.LockResource(timeout);
    }

    public void LockResource(int timeoutMilliseconds)
    {
        _serial.LockResource(timeoutMilliseconds);
    }

    public string LockResource(TimeSpan timeout, string sharedKey)
    {
        return _serial.LockResource(timeout, sharedKey);
    }

    public string LockResource(int timeoutMilliseconds, string sharedKey)
    {
        return _serial.LockResource(timeoutMilliseconds, sharedKey);
    }

    public void UnlockResource()
    {
        _serial.UnlockResource();
    }

    public byte[] ReadRaw(char? toChar = null)
    {
        var en = _serial.TerminationCharacterEnabled;
        _serial.TerminationCharacterEnabled = false;

        try
        {
            byte[] bytes = null;


            var readTermination = _serial.ReadTermination;
            _serial.ReadTermination = SerialTerminationMethod.None;

            //var timeout = _interface.TimeoutMilliseconds;
            //_interface.TimeoutMilliseconds = 1000;//must be greater than read tmo ms this assures that.
            try
            {
                using (var memStream = new MemoryStream())
                {
                    while (true)
                    {
                        try
                        {
                            bytes = null;
                            bytes = _serial.RawIO.Read(1);
                            memStream.Write(bytes, 0, bytes.Length);
                            if (toChar is char && bytes[0] == toChar)
                            {
                                return memStream.ToArray();
                            }
                        }
                        catch (IOTimeoutException e)
                        {
                            if (bytes is null)
                            {
                                if (memStream.Length == 0)
                                {
                                    throw new IOTimeoutException(0, null, "Failed to get anything, timeout " + _serial.TimeoutMilliseconds, e);
                                }
                                return memStream.ToArray();
                            }
                        }
                    }
                }
            }
            finally
            {
                //_interface.TimeoutMilliseconds = timeout;
                _serial.ReadTermination = readTermination;
            }
        }
        finally
        {
            _serial.TerminationCharacterEnabled = en;
        }
    }

    public byte[] Read()
    {
        _interface.SendInterfaceCommand("++read eoi");
        return ReadRaw();
    }

    public byte[] Read(long count)
    {
        throw new NotImplementedException();
    }

    public byte[] Read(long count, out ReadStatus readStatus)
    {
        throw new NotImplementedException();
    }

    public void Read(byte[] buffer, long index, long count, out long actualCount, out ReadStatus readStatus)
    {
        throw new NotImplementedException();
    }

    public unsafe void Read(byte* buffer, long index, long count, out long actualCount, out ReadStatus readStatus)
    {
        throw new NotImplementedException();
    }

    public string ReadString()
    {
        _interface.SendInterfaceCommand("++read");
        return _serial.RawIO.ReadString();
    }

    public string ReadString(long count)
    {
        throw new NotImplementedException();
    }

    public string ReadString(long count, out ReadStatus readStatus)
    {
        throw new NotImplementedException();
    }

    private static readonly byte[] ESC = { 0x1B };
    public void Write(byte[] buffer)
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            switch (buffer[i])
            {
                case (byte)'\n':
                case (byte)'\r':
                case (byte)'+':
                case (byte)'*':
                case 0x1B:
                    _serial.RawIO.Write(ESC);
                    _serial.RawIO.Write(buffer, i, 1);
                    break;
                default:
                    _serial.RawIO.Write(buffer, i, 1);
                    break;
            }
        }
        _serial.RawIO.Write("\n");
        Thread.Sleep(10);
    }

    public void Write(byte[] buffer, long index, long count)
    {
        throw new NotImplementedException();
    }

    public unsafe void Write(byte* buffer, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void Write(string buffer)
    {
        var str = buffer
            .Replace("" + (char)0x1B, "" + (char)0x1B + (char)0x1B)
            .Replace("+", (char)0x1B + "+")
            .Replace("*", (char)0x1B + "*")
            .Replace("\r", (char)0x1B + "\r")
            .Replace("\n", (char)0x1B + "\n");
        _serial.RawIO.Write(str + "\n");
        Thread.Sleep(10);
    }

    public void Write(string buffer, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

internal class AR488Formatted : IMessageBasedFormattedIO, IVisaSession, IDisposable
{
    private IVisaSession _session;
    private IMessageBasedRawIO _raw;
    private AR488Interface _interface;

    public AR488Formatted(IVisaSession session, IMessageBasedRawIO raw)
    {
        _session = session;
        _raw = raw;
        if (session is AR488Interface)
        {
            _interface = session as AR488Interface;
        }
        else if (session is AR488Session)
        {
            _interface = (session as AR488Session).Interface;
        }
    }

    public int ReadBufferSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int WriteBufferSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public void DiscardBuffers()
    {
        throw new NotImplementedException();
    }

    public BinaryEncoding BinaryEncoding { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public ITypeFormatter TypeFormatter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public int EventQueueCapacity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public string HardwareInterfaceName => _session.HardwareInterfaceName;
    public short HardwareInterfaceNumber => _session.HardwareInterfaceNumber;
    public HardwareInterfaceType HardwareInterfaceType => _session.HardwareInterfaceType;
    public string ResourceClass => _session.ResourceClass;
    public Version ResourceImplementationVersion => _session.ResourceImplementationVersion;
    public ResourceLockState ResourceLockState => _session.ResourceLockState;
    public short ResourceManufacturerId => _session.ResourceManufacturerId;
    public string ResourceManufacturerName => _session.ResourceManufacturerName;
    public string ResourceName => _session.ResourceName;
    public Version ResourceSpecificationVersion => _session.ResourceSpecificationVersion;

    public bool SynchronizeCallbacks { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public int TimeoutMilliseconds { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void DisableEvent(EventType eventType)
    {
        throw new NotImplementedException();
    }

    public void DiscardEvents(EventType eventType)
    {
        throw new NotImplementedException();
    }

    public void EnableEvent(EventType eventType)
    {
        throw new NotImplementedException();
    }

    public void FlushWrite(bool sendEnd)
    {
        throw new NotImplementedException();
    }

    public void LockResource()
    {
        throw new NotImplementedException();
    }

    public void LockResource(TimeSpan timeout)
    {
        throw new NotImplementedException();
    }

    public void LockResource(int timeoutMilliseconds)
    {
        throw new NotImplementedException();
    }

    public string LockResource(TimeSpan timeout, string sharedKey)
    {
        throw new NotImplementedException();
    }

    public string LockResource(int timeoutMilliseconds, string sharedKey)
    {
        throw new NotImplementedException();
    }

    public void Printf(string data)
    {
        throw new NotImplementedException();
    }

    public void Printf(string format, params object[] args)
    {
        throw new NotImplementedException();
    }

    public void PrintfAndFlush(string data)
    {
        throw new NotImplementedException();
    }

    public void PrintfAndFlush(string format, params object[] args)
    {
        throw new NotImplementedException();
    }

    public unsafe void PrintfArray(string format, byte* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public unsafe void PrintfArray(string format, sbyte* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public unsafe void PrintfArray(string format, short* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public unsafe void PrintfArray(string format, ushort* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public unsafe void PrintfArray(string format, int* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public unsafe void PrintfArray(string format, uint* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public unsafe void PrintfArray(string format, long* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public unsafe void PrintfArray(string format, ulong* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public unsafe void PrintfArray(string format, float* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public unsafe void PrintfArray(string format, double* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public unsafe void PrintfArrayAndFlush(string format, byte* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public unsafe void PrintfArrayAndFlush(string format, sbyte* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public unsafe void PrintfArrayAndFlush(string format, short* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public unsafe void PrintfArrayAndFlush(string format, ushort* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public unsafe void PrintfArrayAndFlush(string format, int* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public unsafe void PrintfArrayAndFlush(string format, uint* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public unsafe void PrintfArrayAndFlush(string format, long* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public unsafe void PrintfArrayAndFlush(string format, ulong* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public unsafe void PrintfArrayAndFlush(string format, float* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public unsafe void PrintfArrayAndFlush(string format, double* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public byte[] ReadBinaryBlockOfByte()
    {
        throw new NotImplementedException();
    }

    public byte[] ReadBinaryBlockOfByte(bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public long ReadBinaryBlockOfByte(byte[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public long ReadBinaryBlockOfByte(byte[] data, long index, long count, bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public double[] ReadBinaryBlockOfDouble()
    {
        throw new NotImplementedException();
    }

    public double[] ReadBinaryBlockOfDouble(bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public long ReadBinaryBlockOfDouble(double[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public long ReadBinaryBlockOfDouble(double[] data, long index, long count, bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public short[] ReadBinaryBlockOfInt16()
    {
        throw new NotImplementedException();
    }

    public short[] ReadBinaryBlockOfInt16(bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public long ReadBinaryBlockOfInt16(short[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public long ReadBinaryBlockOfInt16(short[] data, long index, long count, bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public int[] ReadBinaryBlockOfInt32()
    {
        throw new NotImplementedException();
    }

    public int[] ReadBinaryBlockOfInt32(bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public long ReadBinaryBlockOfInt32(int[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public long ReadBinaryBlockOfInt32(int[] data, long index, long count, bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public long[] ReadBinaryBlockOfInt64()
    {
        throw new NotImplementedException();
    }

    public long ReadBinaryBlockOfInt64(long[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public sbyte[] ReadBinaryBlockOfSByte()
    {
        throw new NotImplementedException();
    }

    public sbyte[] ReadBinaryBlockOfSByte(bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public long ReadBinaryBlockOfSByte(sbyte[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public long ReadBinaryBlockOfSByte(sbyte[] data, long index, long count, bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public float[] ReadBinaryBlockOfSingle()
    {
        throw new NotImplementedException();
    }

    public float[] ReadBinaryBlockOfSingle(bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public long ReadBinaryBlockOfSingle(float[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public long ReadBinaryBlockOfSingle(float[] data, long index, long count, bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public ushort[] ReadBinaryBlockOfUInt16()
    {
        throw new NotImplementedException();
    }

    public ushort[] ReadBinaryBlockOfUInt16(bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public long ReadBinaryBlockOfUInt16(ushort[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public long ReadBinaryBlockOfUInt16(ushort[] data, long index, long count, bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public uint[] ReadBinaryBlockOfUInt32()
    {
        throw new NotImplementedException();
    }

    public uint[] ReadBinaryBlockOfUInt32(bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public long ReadBinaryBlockOfUInt32(uint[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public long ReadBinaryBlockOfUInt32(uint[] data, long index, long count, bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public ulong[] ReadBinaryBlockOfUInt64()
    {
        throw new NotImplementedException();
    }

    public long ReadBinaryBlockOfUInt64(ulong[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public char ReadChar()
    {
        throw new NotImplementedException();
    }

    public double ReadDouble()
    {
        throw new NotImplementedException();
    }

    public long ReadInt64()
    {
        throw new NotImplementedException();
    }

    public string ReadLine()
    {
        throw new NotImplementedException();
    }

    public int ReadLine(StringBuilder data)
    {
        throw new NotImplementedException();
    }

    public byte[] ReadLineBinaryBlockOfByte()
    {
        throw new NotImplementedException();
    }

    public byte[] ReadLineBinaryBlockOfByte(bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public long ReadLineBinaryBlockOfByte(byte[] data, long index)
    {
        throw new NotImplementedException();
    }

    public long ReadLineBinaryBlockOfByte(byte[] data, long index, bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public double[] ReadLineBinaryBlockOfDouble()
    {
        throw new NotImplementedException();
    }

    public double[] ReadLineBinaryBlockOfDouble(bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public long ReadLineBinaryBlockOfDouble(double[] data, long index)
    {
        throw new NotImplementedException();
    }

    public long ReadLineBinaryBlockOfDouble(double[] data, long index, bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public short[] ReadLineBinaryBlockOfInt16()
    {
        throw new NotImplementedException();
    }

    public short[] ReadLineBinaryBlockOfInt16(bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public long ReadLineBinaryBlockOfInt16(short[] data, long index)
    {
        throw new NotImplementedException();
    }

    public long ReadLineBinaryBlockOfInt16(short[] data, long index, bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public int[] ReadLineBinaryBlockOfInt32()
    {
        throw new NotImplementedException();
    }

    public int[] ReadLineBinaryBlockOfInt32(bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public long ReadLineBinaryBlockOfInt32(int[] data, long index)
    {
        throw new NotImplementedException();
    }

    public long ReadLineBinaryBlockOfInt32(int[] data, long index, bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public long[] ReadLineBinaryBlockOfInt64()
    {
        throw new NotImplementedException();
    }

    public long ReadLineBinaryBlockOfInt64(long[] data, long index)
    {
        throw new NotImplementedException();
    }

    public sbyte[] ReadLineBinaryBlockOfSByte()
    {
        throw new NotImplementedException();
    }

    public sbyte[] ReadLineBinaryBlockOfSByte(bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public long ReadLineBinaryBlockOfSByte(sbyte[] data, long index)
    {
        throw new NotImplementedException();
    }

    public long ReadLineBinaryBlockOfSByte(sbyte[] data, long index, bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public float[] ReadLineBinaryBlockOfSingle()
    {
        throw new NotImplementedException();
    }

    public float[] ReadLineBinaryBlockOfSingle(bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public long ReadLineBinaryBlockOfSingle(float[] data, long index)
    {
        throw new NotImplementedException();
    }

    public long ReadLineBinaryBlockOfSingle(float[] data, long index, bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public ushort[] ReadLineBinaryBlockOfUInt16()
    {
        throw new NotImplementedException();
    }

    public ushort[] ReadLineBinaryBlockOfUInt16(bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public long ReadLineBinaryBlockOfUInt16(ushort[] data, long index)
    {
        throw new NotImplementedException();
    }

    public long ReadLineBinaryBlockOfUInt16(ushort[] data, long index, bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public uint[] ReadLineBinaryBlockOfUInt32()
    {
        throw new NotImplementedException();
    }

    public uint[] ReadLineBinaryBlockOfUInt32(bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public long ReadLineBinaryBlockOfUInt32(uint[] data, long index)
    {
        throw new NotImplementedException();
    }

    public long ReadLineBinaryBlockOfUInt32(uint[] data, long index, bool seekToBlock)
    {
        throw new NotImplementedException();
    }

    public ulong[] ReadLineBinaryBlockOfUInt64()
    {
        throw new NotImplementedException();
    }

    public long ReadLineBinaryBlockOfUInt64(ulong[] data, long index)
    {
        throw new NotImplementedException();
    }

    public char ReadLineChar()
    {
        throw new NotImplementedException();
    }

    public double ReadLineDouble()
    {
        throw new NotImplementedException();
    }

    public long ReadLineInt64()
    {
        throw new NotImplementedException();
    }

    public byte[] ReadLineListOfByte()
    {
        throw new NotImplementedException();
    }

    public long ReadLineListOfByte(byte[] data, long index)
    {
        throw new NotImplementedException();
    }

    public double[] ReadLineListOfDouble()
    {
        throw new NotImplementedException();
    }

    public long ReadLineListOfDouble(double[] data, long index)
    {
        throw new NotImplementedException();
    }

    public short[] ReadLineListOfInt16()
    {
        throw new NotImplementedException();
    }

    public long ReadLineListOfInt16(short[] data, long index)
    {
        throw new NotImplementedException();
    }

    public int[] ReadLineListOfInt32()
    {
        throw new NotImplementedException();
    }

    public long ReadLineListOfInt32(int[] data, long index)
    {
        throw new NotImplementedException();
    }

    public long[] ReadLineListOfInt64()
    {
        throw new NotImplementedException();
    }

    public long ReadLineListOfInt64(long[] data, long index)
    {
        throw new NotImplementedException();
    }

    public sbyte[] ReadLineListOfSByte()
    {
        throw new NotImplementedException();
    }

    public long ReadLineListOfSByte(sbyte[] data, long index)
    {
        throw new NotImplementedException();
    }

    public float[] ReadLineListOfSingle()
    {
        throw new NotImplementedException();
    }

    public long ReadLineListOfSingle(float[] data, long index)
    {
        throw new NotImplementedException();
    }

    public ushort[] ReadLineListOfUInt16()
    {
        throw new NotImplementedException();
    }

    public long ReadLineListOfUInt16(ushort[] data, long index)
    {
        throw new NotImplementedException();
    }

    public uint[] ReadLineListOfUInt32()
    {
        throw new NotImplementedException();
    }

    public long ReadLineListOfUInt32(uint[] data, long index)
    {
        throw new NotImplementedException();
    }

    public ulong[] ReadLineListOfUInt64()
    {
        throw new NotImplementedException();
    }

    public long ReadLineListOfUInt64(ulong[] data, long index)
    {
        throw new NotImplementedException();
    }

    public ulong ReadLineUInt64()
    {
        throw new NotImplementedException();
    }

    public byte[] ReadListOfByte(long count)
    {
        throw new NotImplementedException();
    }

    public long ReadListOfByte(byte[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public double[] ReadListOfDouble(long count)
    {
        throw new NotImplementedException();
    }

    public long ReadListOfDouble(double[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public short[] ReadListOfInt16(long count)
    {
        throw new NotImplementedException();
    }

    public long ReadListOfInt16(short[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public int[] ReadListOfInt32(long count)
    {
        throw new NotImplementedException();
    }

    public long ReadListOfInt32(int[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public long[] ReadListOfInt64(long count)
    {
        throw new NotImplementedException();
    }

    public long ReadListOfInt64(long[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public sbyte[] ReadListOfSByte(long count)
    {
        throw new NotImplementedException();
    }

    public long ReadListOfSByte(sbyte[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public float[] ReadListOfSingle(long count)
    {
        throw new NotImplementedException();
    }

    public long ReadListOfSingle(float[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public ushort[] ReadListOfUInt16(long count)
    {
        throw new NotImplementedException();
    }

    public long ReadListOfUInt16(ushort[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public uint[] ReadListOfUInt32(long count)
    {
        throw new NotImplementedException();
    }

    public long ReadListOfUInt32(uint[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public ulong[] ReadListOfUInt64(long count)
    {
        throw new NotImplementedException();
    }

    public long ReadListOfUInt64(ulong[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public string ReadString()
    {
        throw new NotImplementedException();
    }

    public string ReadString(int count)
    {
        throw new NotImplementedException();
    }

    public int ReadString(StringBuilder data)
    {
        throw new NotImplementedException();
    }

    public int ReadString(StringBuilder data, int count)
    {
        throw new NotImplementedException();
    }

    public ulong ReadUInt64()
    {
        throw new NotImplementedException();
    }

    public string ReadUntilEnd()
    {
        throw new NotImplementedException();
    }

    public string ReadUntilMatch(char ch)
    {
        throw new NotImplementedException();
    }

    public string ReadUntilMatch(string characters, bool discardMatch)
    {
        throw new NotImplementedException();
    }

    public string ReadWhileMatch(string characters)
    {
        throw new NotImplementedException();
    }

    public void Scanf<T>(string format, out T output)
    {
        throw new NotImplementedException();
    }

    public void Scanf<T1, T2>(string format, out T1 output1, out T2 output2)
    {
        throw new NotImplementedException();
    }

    public void Scanf<T1, T2, T3>(string format, out T1 output1, out T2 output2, out T3 output3)
    {
        throw new NotImplementedException();
    }

    public void Scanf<T1, T2, T3, T4>(string format, out T1 output1, out T2 output2, out T3 output3, out T4 output4)
    {
        throw new NotImplementedException();
    }

    public void Scanf<T1, T2, T3, T4, T5>(string format, out T1 output1, out T2 output2, out T3 output3, out T4 output4, out T5 output5)
    {
        throw new NotImplementedException();
    }

    public void Scanf<T1, T2, T3, T4, T5, T6>(string format, out T1 output1, out T2 output2, out T3 output3, out T4 output4, out T5 output5, out T6 output6)
    {
        throw new NotImplementedException();
    }

    public void Scanf<T1, T2, T3, T4, T5, T6, T7>(string format, out T1 output1, out T2 output2, out T3 output3, out T4 output4, out T5 output5, out T6 output6, out T7 output7)
    {
        throw new NotImplementedException();
    }

    public void Scanf<T>(string format, long[] inputs, out T output)
    {
        throw new NotImplementedException();
    }

    public void Scanf<T1, T2>(string format, long[] inputs, out T1 output1, out T2 output2)
    {
        throw new NotImplementedException();
    }

    public void Scanf<T1, T2, T3>(string format, long[] inputs, out T1 output1, out T2 output2, out T3 output3)
    {
        throw new NotImplementedException();
    }

    public void Scanf<T1, T2, T3, T4>(string format, long[] inputs, out T1 output1, out T2 output2, out T3 output3, out T4 output4)
    {
        throw new NotImplementedException();
    }

    public void Scanf<T1, T2, T3, T4, T5>(string format, long[] inputs, out T1 output1, out T2 output2, out T3 output3, out T4 output4, out T5 output5)
    {
        throw new NotImplementedException();
    }

    public void Scanf<T1, T2, T3, T4, T5, T6>(string format, long[] inputs, out T1 output1, out T2 output2, out T3 output3, out T4 output4, out T5 output5, out T6 output6)
    {
        throw new NotImplementedException();
    }

    public void Scanf<T1, T2, T3, T4, T5, T6, T7>(string format, long[] inputs, out T1 output1, out T2 output2, out T3 output3, out T4 output4, out T5 output5, out T6 output6, out T7 output7)
    {
        throw new NotImplementedException();
    }

    public unsafe long ScanfArray(string format, byte* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public unsafe long ScanfArray(string format, sbyte* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public unsafe long ScanfArray(string format, short* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public unsafe long ScanfArray(string format, ushort* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public unsafe long ScanfArray(string format, int* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public unsafe long ScanfArray(string format, uint* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public unsafe long ScanfArray(string format, long* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public unsafe long ScanfArray(string format, ulong* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public unsafe long ScanfArray(string format, float* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public unsafe long ScanfArray(string format, double* pArray, params long[] inputs)
    {
        throw new NotImplementedException();
    }

    public void Skip(long count)
    {
        throw new NotImplementedException();
    }

    public void SkipString(string data)
    {
        throw new NotImplementedException();
    }

    public void SkipUntilEnd()
    {
        throw new NotImplementedException();
    }

    public void UnlockResource()
    {
        throw new NotImplementedException();
    }

    public VisaEventArgs WaitOnEvent(EventType eventType)
    {
        throw new NotImplementedException();
    }

    public VisaEventArgs WaitOnEvent(EventType eventType, out EventQueueStatus status)
    {
        throw new NotImplementedException();
    }

    public VisaEventArgs WaitOnEvent(EventType eventType, int timeoutMilliseconds)
    {
        throw new NotImplementedException();
    }

    public VisaEventArgs WaitOnEvent(EventType eventType, TimeSpan timeout)
    {
        throw new NotImplementedException();
    }

    public VisaEventArgs WaitOnEvent(EventType eventType, int timeoutMilliseconds, out EventQueueStatus status)
    {
        throw new NotImplementedException();
    }

    public VisaEventArgs WaitOnEvent(EventType eventType, TimeSpan timeout, out EventQueueStatus status)
    {
        throw new NotImplementedException();
    }

    public void Write(char data)
    {
        throw new NotImplementedException();
    }

    public void Write(string data)
    {
        throw new NotImplementedException();
    }

    public void Write(long data)
    {
        throw new NotImplementedException();
    }

    public void Write(ulong data)
    {
        throw new NotImplementedException();
    }

    public void Write(double data)
    {
        throw new NotImplementedException();
    }

    public void WriteBinary(byte[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteBinary(byte[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteBinary(sbyte[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteBinary(sbyte[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteBinary(short[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteBinary(short[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteBinary(ushort[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteBinary(ushort[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteBinary(int[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteBinary(int[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteBinary(uint[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteBinary(uint[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteBinary(long[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteBinary(long[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteBinary(ulong[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteBinary(ulong[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteBinary(float[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteBinary(float[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteBinary(double[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteBinary(double[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteBinaryAndFlush(byte[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteBinaryAndFlush(byte[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteBinaryAndFlush(sbyte[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteBinaryAndFlush(sbyte[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteBinaryAndFlush(short[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteBinaryAndFlush(short[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteBinaryAndFlush(ushort[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteBinaryAndFlush(ushort[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteBinaryAndFlush(int[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteBinaryAndFlush(int[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteBinaryAndFlush(uint[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteBinaryAndFlush(uint[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteBinaryAndFlush(long[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteBinaryAndFlush(long[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteBinaryAndFlush(ulong[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteBinaryAndFlush(ulong[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteBinaryAndFlush(float[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteBinaryAndFlush(float[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteBinaryAndFlush(double[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteBinaryAndFlush(double[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteLine()
    {
        throw new NotImplementedException();
    }

    public void WriteLine(char data)
    {
        throw new NotImplementedException();
    }

    public void WriteLine(string data)
    {
        throw new NotImplementedException();
    }

    public void WriteLine(long data)
    {
        throw new NotImplementedException();
    }

    public void WriteLine(ulong data)
    {
        throw new NotImplementedException();
    }

    public void WriteLine(double data)
    {
        throw new NotImplementedException();
    }

    public void WriteLineList(byte[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteLineList(byte[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteLineList(sbyte[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteLineList(sbyte[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteLineList(short[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteLineList(short[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteLineList(ushort[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteLineList(ushort[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteLineList(int[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteLineList(int[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteLineList(uint[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteLineList(uint[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteLineList(long[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteLineList(long[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteLineList(ulong[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteLineList(ulong[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteLineList(float[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteLineList(float[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteLineList(double[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteLineList(double[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteList(byte[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteList(byte[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteList(sbyte[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteList(sbyte[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteList(short[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteList(short[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteList(ushort[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteList(ushort[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteList(int[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteList(int[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteList(uint[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteList(uint[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteList(long[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteList(long[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteList(ulong[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteList(ulong[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteList(float[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteList(float[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void WriteList(double[] data)
    {
        throw new NotImplementedException();
    }

    public void WriteList(double[] data, long index, long count)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

internal class AR488Interface : IGpibInterfaceSession, IVisaSession, IDisposable
{
    public ISerialSession Serial { get; private set; }
    public IMessageBasedRawIO RawIO { get; private set; }

    private string _ver, _verReal;
    private Thread _scanner;

    public AR488Interface(ISerialSession serial)
    {
        Serial = serial;
        Serial.BaudRate = 1000000;
        Serial.SetBufferSize(IOBuffers.ReadWrite, 1024 * 1024);
        Serial.TimeoutMilliseconds = 2000;

        RawIO = new AR488Raw(this, Serial);

        Serial.Flush(IOBuffers.ReadWrite, true);

        while (_ver is null)
        {
            SendInterfaceCommand("++ver");
            try
            {
                _ver = ReadInterfaceResponse();
            }
            catch (IOTimeoutException) { }
        }
        SendInterfaceCommand("++ver real");
        _verReal = ReadInterfaceResponse();

        TerminationCharacterEnabled = true;
        TerminationCharacter = (byte)'\n';

        _terminatorChar = TerminationCharacter;
        _paddr = PrimaryAddress;
        _saddr = SecondaryAddress;

        _scanner = new Thread(() =>
        {
            try
            {
                LineState _previousSrq = LineState.Unasserted;
                LineState _actualSrq = LineState.Unasserted;
                while (true)
                {
                    lock (Serial)
                    {
                        _previousSrq = _actualSrq;
                        _actualSrq = SrqState;
                        if (_actualSrq == LineState.Asserted && _previousSrq == LineState.Unasserted)
                        {
                            ServiceRequest.Invoke(this, new VisaEventArgs(EventType.ServiceRequest));
                        }
                        _previousSrq = SrqState;
                    }
                }
            }
            catch (ThreadAbortException) { }
        });
        //_scanner.Start();
    }

    public void SendInterfaceCommand(string command)
    {
        Serial.RawIO.Write(command + "\n");
        Thread.Sleep(10);
    }

    public string ReadInterfaceResponse()
    {
        return Encoding.UTF8.GetString((RawIO as AR488Raw).ReadRaw('\n'));
    }

    public string HardwareInterfaceName => _verReal + " on " + Serial.HardwareInterfaceName;
    public short HardwareInterfaceNumber => Serial.HardwareInterfaceNumber;
    public HardwareInterfaceType HardwareInterfaceType => HardwareInterfaceType.Gpib;

    public string ResourceClass => "INTFC";
    public Version ResourceImplementationVersion => Serial.ResourceImplementationVersion;
    public ResourceLockState ResourceLockState => Serial.ResourceLockState;

    public short ResourceManufacturerId => Serial.ResourceManufacturerId;
    public string ResourceManufacturerName => Serial.ResourceManufacturerName;
    public string ResourceName => "GPIB::INTFC::AR488::" + Serial.ResourceName;

    public Version ResourceSpecificationVersion => Serial.ResourceSpecificationVersion;

    public GpibAddressedState AddressState
    {
        get
        {
            SendInterfaceCommand("++state");
            var state = int.Parse(ReadInterfaceResponse());
            switch (state)
            {
                case 1:
                case 2:
                case 6:
                case 7:
                default:
                    return GpibAddressedState.Unaddressed;
                case 4:
                case 8:
                case 3:
                    return GpibAddressedState.Talker;
                case 5:
                case 9:
                    return GpibAddressedState.Listener;
            }
        }
    }

    public bool AllowDma { get => false; set { } }

    public LineState AtnState
    {
        get
        {
            SendInterfaceCommand("++xdiag 1");
            var state = int.Parse(ReadInterfaceResponse());
            return (state & 128) != 0 ? LineState.Asserted : LineState.Unasserted;
        }
    }
    public LineState NdacState
    {
        get
        {
            SendInterfaceCommand("++xdiag 1");
            var state = int.Parse(ReadInterfaceResponse());
            return (state & 2) != 0 ? LineState.Asserted : LineState.Unasserted;
        }
    }
    public LineState RenState
    {
        get
        {
            SendInterfaceCommand("++xdiag 1");
            var state = int.Parse(ReadInterfaceResponse());
            return (state & 32) != 0 ? LineState.Asserted : LineState.Unasserted;
        }
    }
    public LineState SrqState
    {
        get
        {
            SendInterfaceCommand("++srq");
            var state = int.Parse(ReadInterfaceResponse());
            return state == 0 ? LineState.Unasserted : LineState.Asserted;
        }
    }

    public short HS488CableLength { get => -1; set { } }
    public IOProtocol IOProtocol { get => IOProtocol.Normal; set { } }

    public byte DeviceStatusByte
    {
        get
        {
            SendInterfaceCommand("++status");
            var state = byte.Parse(ReadInterfaceResponse());
            return state;
        }
        set
        {
            SendInterfaceCommand("++status " + value);
        }
    }

    public bool IsControllerInCharge => IsSystemController;
    public bool IsSystemController
    {
        get
        {
            SendInterfaceCommand("++mode");
            var state = int.Parse(ReadInterfaceResponse());
            return state == 0;
        }
        set
        {
            SendInterfaceCommand("++mode " + (value ? "0" : "1"));
            ControllerInCharge.Invoke(this, new GpibControllerInChargeEventArgs(value));
        }
    }

    private short _paddr, _saddr;
    public short PrimaryAddress
    {
        get
        {
            SendInterfaceCommand("++addr");
            var state = short.Parse(ReadInterfaceResponse().Split(' ')[0]);
            return _paddr = state;
        }
        set
        {
            if (_paddr == value) return;
            SendInterfaceCommand("++addr " + value + " " + SecondaryAddress);
        }
    }
    public short SecondaryAddress
    {
        get
        {
            SendInterfaceCommand("++addr");
            try
            {
                var state = short.Parse(ReadInterfaceResponse().Split(' ')?[1] ?? "0");
                return _saddr = state;
            }
            catch (IndexOutOfRangeException)
            {
                return _saddr = 0;
            }
        }
        set
        {
            if (_saddr == value) return;
            SendInterfaceCommand("++addr " + PrimaryAddress + " " + value);
        }
    }

    private bool _sendEnd;
    public bool SendEndEnabled
    {
        get
        {
            SendInterfaceCommand("++eoi");
            var state = int.Parse(ReadInterfaceResponse());
            return _sendEnd = state == 1;
        }
        set
        {
            if (_sendEnd = value) return;
            SendInterfaceCommand("++eoi " + (value ? 1 : 0));
        }
    }

    private byte _terminatorChar;//todo lazy
    private bool _terminatorEnable;//todo lazy
    public byte TerminationCharacter
    {
        get
        {
            SendInterfaceCommand("++eor");
            var state = int.Parse(ReadInterfaceResponse());
            switch (state)
            {
                case 0:
                    return _terminatorChar = (byte)'\n';
                case 1:
                    return _terminatorChar = (byte)'\r';
                case 2:
                    return _terminatorChar = (byte)'\n';
                case 4:
                    return _terminatorChar = (byte)'\r';
                case 5:
                    return _terminatorChar = 0x03;
                case 6:
                    return _terminatorChar = 0x00;
                case 3:
                case 7:
                    return _terminatorChar = 0x00;
                default:
                    throw new Exception("Invalid Termination setting: " + state);
            }
        }
        set
        {
            if (_terminatorChar == value) return;
            switch (value)
            {
                case (byte)'\n':
                    SendInterfaceCommand("++eor 2");
                    break;
                case (byte)'\r':
                    SendInterfaceCommand("++eor 1");
                    break;
                case 0x03:
                    SendInterfaceCommand("++eor 5");
                    break;
                default:
                    throw new Exception("Invalid Termination setting: " + value);
            }
        }
    }
    public bool TerminationCharacterEnabled
    {
        get
        {
            SendInterfaceCommand("++eor");
            var state = int.Parse(ReadInterfaceResponse());
            switch (state)
            {
                case 0:
                case 1:
                case 2:
                case 4:
                case 5:
                case 6:
                    return _terminatorEnable = true;
                case 3:
                case 7:
                    return _terminatorEnable = false;
                default:
                    throw new Exception("Invalid Termination setting: " + state);
            }
        }
        set
        {
            if (_terminatorEnable = value) return;
            if (value)
            {
                _terminatorChar = TerminationCharacter;
                SendInterfaceCommand("++eor 7");
            }
            else
            {
                TerminationCharacter = _terminatorChar;
            }
        }
    }

    public int EventQueueCapacity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool SynchronizeCallbacks { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public int TimeoutMilliseconds
    {
        get
        {
            SendInterfaceCommand("++read_tmo_ms");
            var state = short.Parse(ReadInterfaceResponse());
            return state;
        }
        set
        {
            value = value > 32000 ? 32000 : value;
            value = value < 1 ? 1 : value;
            SendInterfaceCommand($"++read_tmo_ms {value}");
            Serial.TimeoutMilliseconds = value + 50;
        }
    }

    public event EventHandler<GpibControllerInChargeEventArgs> ControllerInCharge;

    public event EventHandler<VisaEventArgs> ServiceRequest;

    public event EventHandler<VisaEventArgs> Cleared;
    public event EventHandler<VisaEventArgs> Listen;
    public event EventHandler<VisaEventArgs> Talk;
    public event EventHandler<VisaEventArgs> Trigger;

    public void AssertTrigger()
    {
        SendInterfaceCommand("++trg");
    }

    public void ControlAtn(AtnMode command)
    {
        throw new NotImplementedException();
    }

    public void PassControl(short primaryAddress)
    {
        PassControl(primaryAddress, 0);
    }
    public void PassControl(short primaryAddress, short secondaryAddress)
    {
        throw new NotImplementedException();
    }

    public int SendCommand(byte[] data)
    {
        RawIO.Write(data);
        Serial.RawIO.Write("\n");
        return data.Length;
    }

    public void SendInterfaceClear()
    {
        if (IsSystemController)
        {
            SendInterfaceCommand("++ifc");
            ControllerInCharge.Invoke(this, new GpibControllerInChargeEventArgs(true));
        }
    }

    public void SendRemoteLocalCommand(GpibInterfaceRemoteLocalMode mode)
    {
        switch (mode)
        {
            case GpibInterfaceRemoteLocalMode.DeassertRen:
                SendInterfaceCommand("++ren 0");
                break;
            case GpibInterfaceRemoteLocalMode.AssertRen:
                SendInterfaceCommand("++ren 1");
                break;
            case GpibInterfaceRemoteLocalMode.LocalLockoutAssertRen:
                SendInterfaceCommand("++ren 1");
                SendInterfaceCommand("++llo");
                break;
        }
    }

    public void EnableEvent(EventType eventType)
    {
        throw new NotImplementedException();
    }
    public void DisableEvent(EventType eventType)
    {
        throw new NotImplementedException();
    }
    public void DiscardEvents(EventType eventType)
    {
        throw new NotImplementedException();
    }
    public VisaEventArgs WaitOnEvent(EventType eventType)
    {
        throw new NotImplementedException();
    }
    public VisaEventArgs WaitOnEvent(EventType eventType, out EventQueueStatus status)
    {
        throw new NotImplementedException();
    }
    public VisaEventArgs WaitOnEvent(EventType eventType, int timeoutMilliseconds)
    {
        throw new NotImplementedException();
    }
    public VisaEventArgs WaitOnEvent(EventType eventType, TimeSpan timeout)
    {
        throw new NotImplementedException();
    }
    public VisaEventArgs WaitOnEvent(EventType eventType, int timeoutMilliseconds, out EventQueueStatus status)
    {
        throw new NotImplementedException();
    }
    public VisaEventArgs WaitOnEvent(EventType eventType, TimeSpan timeout, out EventQueueStatus status)
    {
        throw new NotImplementedException();
    }

    public void LockResource()
    {
        Serial.LockResource();
    }
    public void LockResource(TimeSpan timeout)
    {
        Serial.LockResource(timeout);
    }
    public void LockResource(int timeoutMilliseconds)
    {
        Serial.LockResource(timeoutMilliseconds);
    }
    public string LockResource(TimeSpan timeout, string sharedKey)
    {
        return Serial.LockResource(timeout, sharedKey);
    }
    public string LockResource(int timeoutMilliseconds, string sharedKey)
    {
        return Serial.LockResource(timeoutMilliseconds, sharedKey);
    }
    public void UnlockResource()
    {
        Serial.UnlockResource();
    }

    public void Dispose()
    {
        //_scanner.Abort();
        //Serial.Dispose();
    }
}

internal class AR488Session : IGpibSession, IMessageBasedSession, IVisaSession, IDisposable
{
    public AR488Interface Interface { get; private set; }
    public IMessageBasedRawIO RawIO { get; private set; }
    public IMessageBasedFormattedIO FormattedIO { get; private set; }

    public AR488Session(AR488Interface controllerInterface, short paddr, short saddr)
    {
        Interface = controllerInterface;
        RawIO = new AR488Raw(this, Interface.Serial);
        FormattedIO = new AR488Formatted(this, Interface.RawIO);

        PrimaryAddress = paddr;
        SecondaryAddress = saddr;
    }

    public short PrimaryAddress { get; private set; }
    public short SecondaryAddress { get; private set; }

    public bool SendEndEnabled { get; set; }
    public byte TerminationCharacter { get; set; }
    public bool TerminationCharacterEnabled { get; set; }

    public bool AllowDma { get => false; set { } }

    public LineState RenState => Interface.RenState;

    public bool ReaddressingEnabled { get => true; set { } }
    public bool UnaddressingEnabled { get => true; set { } }

    public void SendRemoteLocalCommand(RemoteLocalMode mode)
    {
        LineState ren;
        switch (mode)
        {
            case RemoteLocalMode.LocalWithoutLockout:
                Interface.SendInterfaceCommand("++ren 1");
                Interface.SendInterfaceCommand("++loc");
                break;
            case RemoteLocalMode.Remote:
                Interface.SendInterfaceCommand("++ren 1");
                break;
            case RemoteLocalMode.RemoteWithLocalLockout:
                Interface.SendInterfaceCommand("++ren 1");
                Interface.SendInterfaceCommand("++llo");
                break;
            case RemoteLocalMode.Local:
                ren = RenState;
                Interface.SendInterfaceCommand("++ren 1");
                Interface.SendInterfaceCommand("++loc");
                if (ren == LineState.Asserted)
                {
                    Interface.SendInterfaceCommand("++ren 1");
                }
                break;
        }
    }
    public void SendRemoteLocalCommand(GpibInstrumentRemoteLocalMode mode)
    {
        LineState ren;
        switch (mode)
        {
            case GpibInstrumentRemoteLocalMode.DeassertRen:
                Interface.SendInterfaceCommand("++ren 0");
                break;
            case GpibInstrumentRemoteLocalMode.AssertRen:
                Interface.SendInterfaceCommand("++ren 1");
                break;
            case GpibInstrumentRemoteLocalMode.GoToLocalDeassertRen:
                Interface.SendInterfaceCommand("++ren 1");
                Interface.SendInterfaceCommand("++loc");
                break;
            case GpibInstrumentRemoteLocalMode.AddressDeviceAssertRen:
                Interface.SendInterfaceCommand("++ren 1");
                break;
            case GpibInstrumentRemoteLocalMode.AddressDeviceSendLocalLockout:
                Interface.SendInterfaceCommand("++ren 1");
                Interface.SendInterfaceCommand("++llo");
                break;
            case GpibInstrumentRemoteLocalMode.GoToLocal:
                ren = RenState;
                Interface.SendInterfaceCommand("++ren 1");
                Interface.SendInterfaceCommand("++loc");
                if (ren == LineState.Asserted)
                {
                    Interface.SendInterfaceCommand("++ren 1");
                }
                break;
        }
    }

    public IOProtocol IOProtocol { get => Interface.IOProtocol; set => Interface.IOProtocol = value; }

    public int EventQueueCapacity { get => Interface.EventQueueCapacity; set => Interface.EventQueueCapacity = value; }

    public string HardwareInterfaceName => Interface.HardwareInterfaceName;
    public short HardwareInterfaceNumber => Interface.HardwareInterfaceNumber;
    public HardwareInterfaceType HardwareInterfaceType => Interface.HardwareInterfaceType;
    public string ResourceClass => "INSTR";
    public Version ResourceImplementationVersion => Interface.ResourceImplementationVersion;
    public ResourceLockState ResourceLockState => Interface.ResourceLockState;
    public short ResourceManufacturerId => Interface.ResourceManufacturerId;
    public string ResourceManufacturerName => Interface.ResourceManufacturerName;
    public string ResourceName => Interface.ResourceName.Replace(Interface.ResourceClass,
        $"{(PrimaryAddress < 10 ? "0" : "") + PrimaryAddress}::{(SecondaryAddress < 10 ? "0" : "") + SecondaryAddress}::{ResourceClass}");
    public Version ResourceSpecificationVersion => Interface.ResourceSpecificationVersion;

    public bool SynchronizeCallbacks { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int TimeoutMilliseconds { get => Interface.TimeoutMilliseconds; set => Interface.TimeoutMilliseconds = value; }

    public event EventHandler<VisaEventArgs> ServiceRequest;

    public void AssertTrigger()
    {
        Interface.AssertTrigger();
    }

    public void Clear()
    {
        Interface.SendInterfaceCommand("++clr");
    }

    public StatusByteFlags ReadStatusByte()
    {
        Interface.SendInterfaceCommand("++spoll");
        var state = byte.Parse(Interface.ReadInterfaceResponse());
        return (StatusByteFlags)state;
    }

    public void DisableEvent(EventType eventType)
    {
        throw new NotImplementedException();
    }
    public void DiscardEvents(EventType eventType)
    {
        throw new NotImplementedException();
    }
    public void EnableEvent(EventType eventType)
    {
        throw new NotImplementedException();
    }

    public VisaEventArgs WaitOnEvent(EventType eventType)
    {
        throw new NotImplementedException();
    }
    public VisaEventArgs WaitOnEvent(EventType eventType, out EventQueueStatus status)
    {
        throw new NotImplementedException();
    }
    public VisaEventArgs WaitOnEvent(EventType eventType, int timeoutMilliseconds)
    {
        throw new NotImplementedException();
    }
    public VisaEventArgs WaitOnEvent(EventType eventType, TimeSpan timeout)
    {
        throw new NotImplementedException();
    }
    public VisaEventArgs WaitOnEvent(EventType eventType, int timeoutMilliseconds, out EventQueueStatus status)
    {
        throw new NotImplementedException();
    }
    public VisaEventArgs WaitOnEvent(EventType eventType, TimeSpan timeout, out EventQueueStatus status)
    {
        throw new NotImplementedException();
    }

    public void LockResource()
    {
        Interface.LockResource();
    }
    public void LockResource(TimeSpan timeout)
    {
        Interface.LockResource(timeout);
    }
    public void LockResource(int timeoutMilliseconds)
    {
        Interface.LockResource(timeoutMilliseconds);
    }
    public string LockResource(TimeSpan timeout, string sharedKey)
    {
        return Interface.LockResource(timeout, sharedKey);
    }
    public string LockResource(int timeoutMilliseconds, string sharedKey)
    {
        return Interface.LockResource(timeoutMilliseconds, sharedKey);
    }
    public void UnlockResource()
    {
        Interface.UnlockResource();
    }

    public void Dispose()
    {
        Interface.Dispose();//wouldnt that cause evilness?
    }
}
