using System.Security;
using System.Text;
using GeneralUnifiedTestSystemYard.Core.Exceptions;
using Newtonsoft.Json.Linq;

namespace GeneralUnifiedTestSystemYard.Core.EntryPoints.CLI;

public class GutsyConsole : IGutsyEntryPoint
{
    public string Identifier => "CLI";
    
    public void Start(GutsyCore gutsy, JToken? token)
    {
        Console.WriteLine("Insert JSON as single line, or file path:");
        while (Console.ReadLine() is {} line)
        {
            try
            {
                var json = line.Contains('{') ? line : File.ReadAllText(line, Encoding.ASCII);
                Console.WriteLine(gutsy.ProcessJson(json));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}