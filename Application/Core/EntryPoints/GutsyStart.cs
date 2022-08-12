using GeneralUnifiedTestSystemYard.Core.ClassExtensions;
using Newtonsoft.Json;

namespace GeneralUnifiedTestSystemYard.Core.EntryPoints;

public static class GutsyStart
{
    static GutsyStart()
    {
        EntryPoints.LoadFromAssembly();
    }

    public static SortedDictionary<string, IGutsyEntryPoint> EntryPoints { get; } = new();

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
                    EntryPoints[settings.EntryPoint].Start(new GutsyCore(), settings.Settings);
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