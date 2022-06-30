using System.Runtime.InteropServices;
using System.Reflection;
using System.Security;
using Ivi.Visa;
using GeneralUnifiedTestSystemYard.Core;

namespace GeneralUnifiedTestSystemYard.Commands.VISA;

/// <summary>
/// VISA.NET Shared Components 5.11.0 v2.0.50727 (Taken from NI VISA.NET 21.5 v4.0.30319)
/// </summary>
public static class VisaWrapper
{
    private const string wildcard = "?*";
    /// <summary>
    /// Shouldnt be set
    /// </summary>
    public static IResourceManager? ResourceManager { get; private set; }
    public static SortedDictionary<string, IVisaResourceManagerSupplier> ManagerSuppliers { get; } = new();
    private static SortedDictionary<string, IVisaSessionResolver> SessionResolvers { get; } = new();
    private static SortedDictionary<string, IVisaDeviceResolver> DeviceResolvers { get; } = new();

    /// <exception cref="DllNotFoundException"></exception>
    static VisaWrapper()
    {
        ManagerSuppliers.LoadFromFolder("*GUTSY VISA Resource Manager*.dll");
        SessionResolvers.LoadFromFolder("*GUTSY VISA Session Resolver*.dll");
        DeviceResolvers.LoadFromFolder("*GUTSY VISA Device*.dll");

        foreach (var managerSupplier in ManagerSuppliers.Values)
        {
            if(managerSupplier.GetResourceManager() is IResourceManager resourceManager)
            {
                ResourceManager = resourceManager;
                break;
            }
        }

        if(ResourceManager is null)
        {
            throw new DllNotFoundException("Could not find a working VISA resource manager");
        }
    }

    /// <exception cref="AggregateException"></exception>
    public static SortedSet<string> FindDevices(IResourceManager resourceManager,string query = wildcard)
    {
        List<string> list = new();
        var found = resourceManager.Find(query);

        var p = Parallel.ForEach(found, item => {
            list.AddRange(ResolveSessionAbstractionLayer(item));
        });

        while (!p.IsCompleted) { }

        return new SortedSet<string>(list);
    }

    public static Hardware Open(string resource, AccessModes mode = AccessModes.None, int openTimeout = 1000)
    {
        if (resource is string)
        {
            foreach (var item in SessionResolvers)
            {
                try
                {
                    var session = item.Invoke(resourceManager, resource);
                    if (session is IVisaSession)
                    {
                        return new Hardware(resource, session);
                    }
                }
                catch (Exception e) { }
            }
        }
        return new Hardware(resource, resourceManager.Open(resource, mode, openTimeout));
    }

    public static List<string> ResolveSessionAbstractionLayer(string resource)
    {
        List<string> sessionsGenerated = new List<string>();
        if (resource is string)
        {
            foreach (var generator in SessionFinderRegistry)
            {
                var strings = generator.Invoke(resourceManager, resource);
                if (strings is List<string>)
                {
                    sessionsGenerated.AddRange(strings);
                }
            }
        }
        if (sessionsGenerated.Count == 0)
        {
            sessionsGenerated.Add(resource);
        }
        return sessionsGenerated;
    }

    public static List<IDevice> ResolveDeviceAbstractionLayer(Hardware resource)
    {
        List<IDevice> devicesGenerated = new List<IDevice>();
        if (resource.Valid)
        {
            foreach (var generator in Devices)
            {
                var device = generator.Invoke(resource);
                if (device is IDevice)
                {
                    devicesGenerated.Add(device);
                }
            }
        }
        return devicesGenerated;
    }
}
