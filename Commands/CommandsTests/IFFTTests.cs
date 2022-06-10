using GeneralUnifiedTestSystemYard.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Numerics;

namespace GeneralUnifiedTestSystemYard.Commands.CommandsTests;

[TestClass()]
public class IFFTTests
{
    /// <exception cref="OverflowException"></exception>
    [TestMethod()]
    public void ExecuteTest()
    {
        var command = GUTSY.GetCommandByName("IFFT");

        var input = new List<Complex>();
        for (int i = 0; i < 1025; i++)
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