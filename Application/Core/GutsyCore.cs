using System.Security;
using GeneralUnifiedTestSystemYard.Core.ClassExtensions;
using GeneralUnifiedTestSystemYard.Core.Command;
using GeneralUnifiedTestSystemYard.Core.Exceptions;
using GeneralUnifiedTestSystemYard.Core.Module;
using GeneralUnifiedTestSystemYard.Core.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace GeneralUnifiedTestSystemYard.Core;

public class GutsyCore
{
    /// <summary>
    ///     Sets state of the JSON converter, to be more human readable.
    ///     Also to enforce Naming Strategy to Camel Case and enums to strings.
    /// </summary>
    public static JsonSerializerSettings JsonSerializerSettings { get; } = new()
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

    public SortedDictionary<string, IGutsyCommand> Commands { get; } = new();
    public SortedDictionary<string, IGutsyModule> Extensions { get; } = new();

    /// <exception cref="IOException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="SecurityException"></exception>
    /// <exception cref="ExtensionException"></exception>
    public GutsyCore()
    {
        Commands.LoadFromFolder("*GUTSY Command*");
        Extensions.LoadFromFolder("*GUTSY Extension*");
        foreach (var extension in Extensions.Values)
        {
            try
            {
                extension.Activate(this);
            }
            catch (Exception e)
            {
                throw new ExtensionException($"Failed to activate extension: {extension.Identifier}", e);
            }
        }
    }

    /// <summary>
    ///     Just a string wrapper for Process based on objects, allows direct JSON string IO.
    /// </summary>
    /// <param name="requestJson"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    public string ProcessJson(string? requestJson, JsonSerializerSettings? settings = null)
    {
        var serializationSettings = settings ?? JsonSerializerSettings;
        GutsyResponse response;
        if (requestJson != null)
        {
            requestJson = requestJson.Trim();
            if (requestJson.Length > 0)
                try
                {
                    var request = JsonConvert.DeserializeObject<GutsyRequest?>(requestJson, serializationSettings);
                    response = Process(request);
                }
                catch (Exception e)
                {
                    response = new GutsyResponse
                    {
                        Exception = new RequestUndefinedException("Cannot parse request", e)
                    };
                }
            else
            {
                response = Process(null);
            }
        }
        else
        {
            response = Process(null);
        }

        return JsonConvert.SerializeObject(response, serializationSettings);
    }

    private GutsyResponse Process(GutsyRequest? request)
    {
        var response = new GutsyResponse();
        if (request == null) return response;

        response.Command = request.Command;
        response.Guid = request.Guid;

        if (request.Command is { } commandId)
        {
            if (Commands.GetFirstByName(commandId) is { } command)
            {
                try
                {
                    //response.Parameters = request.Parameters; // Don't send input back
                    response.Result = command.Execute(request.Parameters);
                }
                catch (Exception e)
                {
                    response.Parameters = request.Parameters;
                    response.Exception = new CommandFailedException("Failed to run command", e);
                }
            }
            else
            {
                response.Parameters = request.Parameters;
                response.Exception = new CommandUndefinedException($"Missing command with name: {commandId}");
            }
        }
        else
        {
            response.Parameters = request.Parameters;
            response.Exception = new CommandUndefinedException("Command Name unspecified");
        }

        return response;
    }
}