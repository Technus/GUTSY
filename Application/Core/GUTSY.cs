using GeneralUnifiedTestSystemYard.Core.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Runtime.Serialization;
using System.Security;

namespace GeneralUnifiedTestSystemYard.Core;

public static class GUTSY
{
    public static SortedDictionary<string, IGUTSYCommand> Commands { get; } = new();
    public static SortedDictionary<string, IGUTSYExtension> Extensions { get; } = new();

    /// <exception cref="IOException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="SecurityException"></exception>
    static GUTSY()
    {
        InitializeJsonConvert();
        Commands.LoadFromFolder("*GUTSY Command*.dll");
        Commands.LoadFromFolder("*GUTSY Extension*.dll");
    }

    public static void InitializeJsonConvert()
    {
        JsonConvert.DefaultSettings = () => new()
        {
            Formatting = Formatting.Indented,
            Converters =
            {
                new ComplexJsonConverter(),
            },
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy
                {
                    ProcessDictionaryKeys = false,
                },
            },
        };
    }

    /// <summary>
    /// Just a string wrapper for Process based on objects, allows direct JSON string IO.
    /// </summary>
    /// <param name="requestStringMaybe"></param>
    /// <returns></returns>
    public static string ProcessJSON(string? requestStringMaybe)
    {
        GUTSYResponse response;
        if (requestStringMaybe is string requestString && requestString.Trim().Length>0)
        {
            try
            {
                var requestMaybe = JsonConvert.DeserializeObject<GUTSYRequest?>(requestString);
                response = Process(requestMaybe);
            }
            catch (Exception e)
            {
                response = new()
                {
                    Exception = new RequestUndefinedException("Cannot parse request", e)
                };
            }
        }
        else
        {
            response = Process(null);
        }
        return JsonConvert.SerializeObject(response);
    }

    public static GUTSYResponse Process(GUTSYRequest? requestMaybe)
    {
        var response = new GUTSYResponse();
        if (requestMaybe is GUTSYRequest request)
        {
            response.Command = request.Command;
            response.Guid = request.Guid;

            if (request.Command is string commandID)
            {
                if (Commands.GetFirstByName(commandID) is IGUTSYCommand command)
                {
                    try
                    {
                        //response.Parameters = request.Parameters; // Don't send input back
                        response.Result = command.Execute(request.Parameters);
                    }
                    catch (Exception e)
                    {
                        response.Parameters = request.Parameters;
                        response.Exception = new CommandFailedExcpetion($"Failed to run command",e);
                    }
                }
                else
                {
                    response.Parameters = request.Parameters;
                    response.Exception = new CommandUndefinedException($"Missing command with name: {commandID}");
                }
            }
            else
            {
                response.Parameters = request.Parameters;
                response.Exception = new CommandUndefinedException("Command Name unspecified");
            }
        }
        return response;
    }
}

public class RequestUndefinedException : Exception
{
    public RequestUndefinedException()
    {
    }

    public RequestUndefinedException(string? message) : base(message)
    {
    }

    public RequestUndefinedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected RequestUndefinedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

public class CommandUndefinedException : Exception
{
    public CommandUndefinedException()
    {
    }

    public CommandUndefinedException(string? message) : base(message)
    {
    }

    public CommandUndefinedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected CommandUndefinedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

public class CommandFailedExcpetion : Exception
{
    public CommandFailedExcpetion()
    {
    }

    public CommandFailedExcpetion(string? message) : base(message)
    {
    }

    public CommandFailedExcpetion(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected CommandFailedExcpetion(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
