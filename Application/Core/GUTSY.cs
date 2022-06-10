using GeneralUnifiedTestSystemYard.Core.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace GeneralUnifiedTestSystemYard.Core;

public static class GUTSY
{
    static GUTSY()
    {
        JsonConvert.DefaultSettings = () => new()
        {
            Formatting = Formatting.Indented,
            Converters =
            {
                new ComplexJsonConverter(),
            },
        };

        LoadCommandsFromFolder();
    }

    public static Dictionary<string, IGUTSYCommand> Commands { get; } = new();

    /// <exception cref="IOException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="SecurityException"></exception>
    public static void LoadCommandsFromFolder(string path=".")
    {
        if(Directory.Exists(path))
        {
            foreach (var file in Directory.EnumerateFiles(path, "* GUTSY Commands.dll"))
            {
                LoadCommandsFromDLL(file);
            }
            foreach (var dir in Directory.EnumerateDirectories(path))
            {
                LoadCommandsFromFolder(dir);
            }
        }
    }

    /// <exception cref="FileLoadException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="BadImageFormatException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="TargetInvocationException"></exception>
    /// <exception cref="MethodAccessException"></exception>
    /// <exception cref="MemberAccessException"></exception>
    /// <exception cref="InvalidComObjectException"></exception>
    /// <exception cref="COMException"></exception>
    /// <exception cref="TypeLoadException"></exception>
    /// <exception cref="SecurityException"></exception>
    /// <exception cref="PathTooLongException"></exception>
    public static void LoadCommandsFromDLL(string path)
    {
        foreach (var type in Assembly.LoadFile(Path.GetFullPath(path)).GetExportedTypes())
        {
            if (type.IsAssignableTo(typeof(IGUTSYCommand)))
            {
                if (Activator.CreateInstance(type) is IGUTSYCommand command)
                {
                    Commands.Add(command.GetID(), command);
                }
            }
        }
    }

    public static IGUTSYCommand GetCommandByName(string name)
    {
        return Commands.Where(kv => kv.Key.Equals(name)).Select(kv => kv.Value).First();
    }

    public static Dictionary<string, IGUTSYCommand> GetCommandsByName(string name)
    {
        return new(Commands.Where(kv => kv.Key.EndsWith(name)).Select(kv => kv.Value).ToDictionary(v => v.GetID()));
    }

    public static Dictionary<string, IGUTSYCommand> GetCommandsInPath(string path)
    {
        return new(Commands.Where(kv => kv.Key.StartsWith(path) && !kv.Key[path.Length..].Contains('.')).Select(kv => kv.Value).ToDictionary(v => v.GetID()));
    }

    public static Dictionary<string, IGUTSYCommand> GetCommandsInPathRecursive(string path)
    {
        return new(Commands.Where(kv => kv.Key.StartsWith(path)).Select(kv=>kv.Value).ToDictionary(v=>v.GetID()));
    }

    /// <exception cref="JsonReaderException"></exception>
    public static JObject Process(JObject? requestMaybe)
    {
        var response = new JObject();
        if (requestMaybe is JObject request)
        {
            if (request?["command"]?.Value<string>() is string commandID)
            {
                if (GetCommandByName(commandID) is IGUTSYCommand command)
                {
                    response["command"] = request["command"];
                    try
                    {
                        if (command.Execute(request["parameters"]) is JToken token)
                        {
                            response["result"] = token;
                        }
                        else
                        {
                            response["result"] = null;
                        }
                    }
                    catch (Exception e)
                    {
                        response["error"] = JToken.FromObject($"Failed to run command: {commandID}");
                        response["exception"] = JToken.FromObject(e);
                        response["request"] = request;
                    }
                }
                else
                {
                    response["error"] = JToken.FromObject($"Missing command with name: {commandID}");
                    response["request"] = request;
                }
            }
            else
            {
                response["error"] = JToken.FromObject($"Missing command name");
                response["request"] = request;
            }
        }
        else
        {
            response["error"] = JToken.FromObject($"Missing request");
        }

        return response;
    }
}