using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Numerics;

namespace GeneralUnifiedTestSystemYard.Core.Tests;

[TestClass()]
public class GUTSYTests
{
    [TestMethod()]
    public void TestJsonArray()
    {
        var str = @"{'val':[0,3]}";
        dynamic json = JObject.Parse(str);
        Console.WriteLine(json.val.ToObject<double[]>());
    }

    [TestMethod()]
    public void TestJsonComplex()
    {
        Complex complex = new(2,3);
        dynamic json = JObject.FromObject(complex);
        Console.WriteLine(json);
    }
}