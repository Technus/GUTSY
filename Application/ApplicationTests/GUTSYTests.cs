using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace ApplicationTests;

[TestClass]
public class GutsyTests
{
    [TestMethod]
    public void TestJsonArray()
    {
        const string str = @"{'val':[0,3]}";
        dynamic json = JObject.Parse(str);
        Console.WriteLine(json.val.ToObject<double[]>());
    }

    [TestMethod]
    public void TestJsonComplex()
    {
        Complex complex = new(2,3);
        dynamic json = JObject.FromObject(complex);
        Console.WriteLine(json);
    }
}