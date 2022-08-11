using System.Numerics;
using GeneralUnifiedTestSystemYard.Core;
using GeneralUnifiedTestSystemYard.Core.ClassExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace CommandsTests;

[TestClass]
public class FftTests
{
    private GutsyCore Gutsy { get; } = new();

    /// <exception cref="OverflowException"></exception>
    [TestMethod]
    public void ExecuteTest()
    {
        var command = Gutsy.Commands.GetFirstByName("FFT");

        Assert.IsNotNull(command);

        var input = new JArray();
        for (var i = 0; i < 2048; i++)
            input.Add(1 + Math.Sin(i * 2 * Math.PI * 128 / 2048));

        var output = command.Execute(input) as JArray;

        var complex = output?.ToObject<Complex[]>() ?? Array.Empty<Complex>();

        Console.WriteLine(complex);
    }
}