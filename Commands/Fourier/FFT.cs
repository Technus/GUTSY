using FFTW.NET;
using GeneralUnifiedTestSystemYard.Command;
using Newtonsoft.Json.Linq;

namespace GeneralUnifiedTestSystemYard.Commands.Fourier;

public class Fft : IGutsyCommand
{
    public string Identifier => "FFT";

    /// <summary>
    ///     Computes the FFT
    /// </summary>
    /// <param name="parameters">JSON Array of numeric values</param>
    /// <returns>JSON Array of serialized complex values</returns>
    public JToken? Execute(JToken? parameters)
    {
        if (parameters is not JArray array) return null;

        var input = array.ToObject<double[]>() ?? Array.Empty<double>();

        using var pinIn = new PinnedArray<double>(input);
        using var pinOut = new FftwArrayComplex(DFT.GetComplexBufferSize(pinIn.GetSize()));
        DFT.FFT(pinIn, pinOut);

        var result = new JArray
        {
            JToken.FromObject(pinOut[0] / input.LongLength) //DC to RMS
        };

        var scale = Math.Sqrt(2) / input.LongLength; //AC to rms

        for (int i = 1, len = pinOut.Length; i < len; i++) result.Add(JToken.FromObject(pinOut[i] * scale));

        return result;
    }
}