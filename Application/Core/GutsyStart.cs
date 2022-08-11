using GeneralUnifiedTestSystemYard.Core.Networking;
using GeneralUnifiedTestSystemYard.Core.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

using CMD =  System.Console;

namespace GeneralUnifiedTestSystemYard.Core;
public static class GutsyStart
{
    /// <exception cref="IOException"></exception>
    public static int Main(string[] parameters)
    {
        InitializeJsonConvert();
        try
        {
            if (parameters.Length > 0)
            {
                var json = File.ReadAllText(parameters[0]);
                var settings = JsonConvert.DeserializeObject<GutsySettings>(json);

            }
            else
            {
                var server = new GutsyServer();
                CMD.WriteLine("Gutsy Server starting on local host");
                server.StartListeningOnLocalHost();
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to start GUTSY", ex);
        }
        return 0;
    }

    /// <summary>
    /// Sets state of the JSON converter, to be more human readable.
    /// Also to enforce Naming Strategy to Camel Case and enums to strings.
    /// </summary>
    private static void InitializeJsonConvert()
    {
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
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
    }
}
