using GeneralUnifiedTestSystemYard.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Numerics;

namespace GeneralUnifiedTestSystemYard.Commands.CommandsTests;

[TestClass()]
public class FFTTests
{
    /// <exception cref="OverflowException"></exception>
    [TestMethod()]
    public void ExecuteTest()
    {
        var command = GUTSY.GetCommandByName("FFT");

        var input = new JArray();
        for (int i = 0; i < 2048; i++)
            input.Add(1+Math.Sin(i * 2 * Math.PI * 128 / 2048));

        var output = command.Execute(input) as JArray;

        var complex = output?.ToObject<Complex[]>() ?? Array.Empty<Complex>();

        Console.WriteLine(complex);
    }
}