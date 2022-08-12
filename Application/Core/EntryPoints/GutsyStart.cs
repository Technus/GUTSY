using GeneralUnifiedTestSystemYard.Core.ClassExtensions;
using GeneralUnifiedTestSystemYard.Core.EntryPoints.CLI;
using Newtonsoft.Json;

namespace GeneralUnifiedTestSystemYard.Core.EntryPoints;

public static class GutsyStart
{
    public static SortedDictionary<string, IGutsyEntryPoint> EntryPoints { get; } = new();

    static GutsyStart()
    {
        EntryPoints.LoadFromAssembly();
    }
    
    /// <exception cref="IOException"></exception>
    public static void Main(string[] parameters)
    {
        try
        {
            if (parameters.Length > 0)
            {
                var json = File.ReadAllText(parameters[0]);
                if (JsonConvert.DeserializeObject<GutsySettings>(json, GutsyCore.JsonSerializerSettings) is
                    { } settings)
                {
                    EntryPoints[settings.EntryPoint].Start(new GutsyCore(),settings.Settings);
                }
            }
            else
            {
                EntryPoints["CLI"].Start(new GutsyCore());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
}