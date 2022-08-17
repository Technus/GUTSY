using System.Security;
using GeneralUnifiedTestSystemYard.ClassExtensions;
using Ivi.Visa;

namespace GeneralUnifiedTestSystemYard.Commands.VISA;

/// <summary>
///     VISA.NET Shared Components 5.11.0 v2.0.50727 (Taken from NI VISA.NET 21.5 v4.0.30319)
/// </summary>
public class Visa
{
    public const string Wildcard = "?*";

    /// <exception cref="DllNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="SecurityException"></exception>
    public Visa()
    {
        ManagerSuppliers.LoadFromFolder("*GUTSY VISA Resource Manager*");
        SessionResolvers.LoadFromFolder("*GUTSY VISA Session Resolver*");
        DeviceResolvers.LoadFromFolder("*GUTSY VISA Device*");

        foreach (var managerSupplier in ManagerSuppliers.Values)
            if (managerSupplier.GetResourceManager() is { } resourceManager)
            {
                ResourceManager = resourceManager;
                break;
            }

        if (ResourceManager is null) throw new DllNotFoundException("Could not find a working VISA resource manager");
    }

    public IResourceManager ResourceManager { get; }
    public SortedDictionary<string, IVisaResourceManagerSupplier> ManagerSuppliers { get; } = new();
    public SortedDictionary<string, IVisaSessionResolver> SessionResolvers { get; } = new();
    public SortedDictionary<string, IVisaDeviceResolver> DeviceResolvers { get; } = new();

    /// <exception cref="AggregateException"></exception>
    public SortedSet<string> FindDevices(IResourceManager resourceManager, string query = Wildcard)
    {
        var list = new List<string>();
        var found = resourceManager.Find(query);

        var parallel = Parallel.ForEach(found, item => list.AddRange(ResolveSessionAbstractionLayer(item)));

        while (!parallel.IsCompleted)
        {
        }

        return new SortedSet<string>(list);
    }

    public IVisaHardware Open(string resource, AccessModes mode = AccessModes.None, int openTimeout = 1000)
    {
        foreach (var item in SessionResolvers)
            try
            {
                var session = item.Value.ResolveSession(ResourceManager, resource);
                if (session is not null) return new Hardware(resource, session);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        return new Hardware(resource, resourceManager.Open(resource, mode, openTimeout));
    }

    public IEnumerable<string> ResolveSessionAbstractionLayer(string resource)
    {
        var sessionsGenerated = new List<string>();

        foreach (var generator in SessionFinderRegistry)
        {
            var strings = generator.Invoke(resourceManager, resource);
            if (strings is List<string>) sessionsGenerated.AddRange(strings);
        }

        if (sessionsGenerated.Count == 0) sessionsGenerated.Add(resource);
        return sessionsGenerated;
    }

    public List<IVisaDevice> ResolveDeviceAbstractionLayer(IVisaHardware resource)
    {
        var devicesGenerated = new List<IVisaDevice>();
        if (resource.Valid)
            foreach (var generator in Devices)
            {
                var device = generator.Invoke(resource);
                if (device is IVisaDevice) devicesGenerated.Add(device);
            }

        return devicesGenerated;
    }
}