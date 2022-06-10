using FFTW.NET;
using GeneralUnifiedTestSystemYard.Core;
using Newtonsoft.Json.Linq;

namespace GeneralUnifiedTestSystemYard.Commands.Fourier;

public class FFT : IGUTSYCommand
{
    /// <summary>
    /// Computes the FFT
    /// </summary>
    /// <param name="parameters">JSON Array of numeric values</param>
    /// <returns>JSON Array of stringified complex values</returns>
    public JToken Execute(JToken parameters)
    {
        var input = parameters?.ToObject<double[]>() ?? Array.Empty<double>();

        using var pinIn = new PinnedArray<double>(input);
        using var pinOut = new FftwArrayComplex(DFT.GetComplexBufferSize(pinIn.GetSize()));
        DFT.FFT(pinIn, pinOut);

        var result = new JArray();

        result.Add(JToken.FromObject(pinOut[0] / input.LongLength));//DC to RMS

        var scale = Math.Sqrt(2) / input.LongLength;//AC to rms

        for (int i = 1, len = pinOut.Length; i < len; i++)
        {
            result.Add(JToken.FromObject(pinOut[i] * scale));
        }

        return result;
    }

    public string GetID()
    {
        return "FFT";
    }
}
