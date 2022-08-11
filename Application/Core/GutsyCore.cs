using System.Security;
using GeneralUnifiedTestSystemYard.Core.ClassExtensions;
using GeneralUnifiedTestSystemYard.Core.Command;
using GeneralUnifiedTestSystemYard.Core.Exceptions;
using GeneralUnifiedTestSystemYard.Core.Extension;
using Newtonsoft.Json;

namespace GeneralUnifiedTestSystemYard.Core;

public class GutsyCore
{
    /// <exception cref="IOException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="SecurityException"></exception>
    /// <exception cref="ExtensionException"></exception>
    public GutsyCore()
    {
        Commands.LoadFromFolder("*GUTSY Command*");
        Extensions.LoadFromFolder("*GUTSY Extension*");
        Extensions.Values.ForEach(extension =>
        {
            try
            {
                extension.Activate(this);
            }
            catch (Exception e)
            {
                throw new ExtensionException($"Failed to activate extension: {extension.Identifier}", e);
            }
        });
    }

    public SortedDictionary<string, IGutsyCommand> Commands { get; } = new();
    public SortedDictionary<string, IGutsyModule> Extensions { get; } = new();

    /// <summary>
    ///     Just a string wrapper for Process based on objects, allows direct JSON string IO.
    /// </summary>
    /// <param name="requestString"></param>
    /// <returns></returns>
    public string ProcessJson(string? requestString)
    {
        GutsyResponse response;
        if (requestString != null && requestString.Trim().Length > 0)
            try
            {
                var request = JsonConvert.DeserializeObject<GutsyRequest?>(requestString);
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
            response = Process(null);

        return JsonConvert.SerializeObject(response);
    }

    public GutsyResponse Process(GutsyRequest? request)
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