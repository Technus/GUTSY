using System.Numerics;
using GeneralUnifiedTestSystemYard.Core;
using GeneralUnifiedTestSystemYard.Core.ClassExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace CommandsTests;

[TestClass]
public class IfftTests
{
    private GutsyCore Gutsy { get; } = new ();
    
    /// <exception cref="OverflowException"></exception>
    [TestMethod]
    public void ExecuteTest()
    {
        var command = Gutsy.Commands.GetFirstByName("IFFT");
        
        Assert.IsNotNull(command);

        var input = new List<Complex>();
        for (var i = 0; i < 1025; i++)
        {
            input.Add(Complex.Zero);
        }
        input[0] = 1;
        input[3] = new Complex(1 / Math.Sqrt(2), 0);

        var inputJson = JToken.FromObject(input);

        var outputJson = command.Execute(inputJson) as JArray;

        var output = outputJson?.ToObject<double[]>();

        Console.WriteLine(output);
    }
}