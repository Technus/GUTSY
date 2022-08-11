using System.Globalization;
using System.Numerics;

namespace GeneralUnifiedTestSystemYard.Core.Numerics;

public static class NumericsExtensions
{
    private const double RadToDeg = 180 / Math.PI;
    private const double DegToRad = Math.PI / 180;

    public static string ToCartesian(this Complex complex)
    {
        return
            $"{complex.Real.ToString(CultureInfo.InvariantCulture)} {complex.Imaginary.ToString(CultureInfo.InvariantCulture)}i";
    }

    public static string ToPolar(this Complex complex, bool useRadians = true)
    {
        return
            $"{complex.Magnitude.ToString(CultureInfo.InvariantCulture)} {(useRadians ? $"{complex.Phase.ToString(CultureInfo.InvariantCulture)}rad" : $"{(complex.Phase * RadToDeg).ToString(CultureInfo.InvariantCulture)}deg")}";
    }

    public static Complex FromCartesian(this string complex)
    {
        var parts = complex.Split(" ");
        return new Complex(double.Parse(parts[0], CultureInfo.InvariantCulture),
            double.Parse(parts[1].ToLowerInvariant().Replace("i", ""), CultureInfo.InvariantCulture));
    }

    /// <exception cref="FormatException"></exception>
    /// <exception cref="OverflowException"></exception>
    public static Complex FromPolar(this string complex)
    {
        var parts = complex.Split(" ");
        var angleType = parts[1].ToLowerInvariant();
        if (angleType.Contains("rad"))
            return Complex.FromPolarCoordinates(double.Parse(parts[0], CultureInfo.InvariantCulture),
                double.Parse(angleType.Replace("rad", ""), CultureInfo.InvariantCulture));
        if (angleType.Contains("deg"))
            return Complex.FromPolarCoordinates(double.Parse(parts[0], CultureInfo.InvariantCulture),
                double.Parse(angleType.Replace("deg", ""), CultureInfo.InvariantCulture) * DegToRad);
        throw new FormatException($"Cannot parse, unknown angle unit: {complex}");
    }
}