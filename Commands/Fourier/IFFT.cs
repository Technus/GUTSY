﻿using FFTW.NET;
using GeneralUnifiedTestSystemYard.Core;
using Newtonsoft.Json.Linq;
using System.Numerics;

namespace GeneralUnifiedTestSystemYard.Commands.Fourier;

public class IFFT : IGUTSYCommand
{
    /// <exception cref="OverflowException"></exception>
    public static int[] GetRealBufferSize(int[] complexBufferSize)
    {
        int[] array = new int[complexBufferSize.Length];
        Buffer.BlockCopy(complexBufferSize, 0, array, 0, array.Length * sizeof(int));
        array[array.Length - 1] = (complexBufferSize[array.Length - 1] - 1) * 2;
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = array[i] < 0 ? 0 : array[i];
        }
        return array;
    }

    /// <summary>
    /// Computes the reverse FFT
    /// </summary>
    /// <param name="array">JSON Array of stringified complex values</param>
    /// <returns>JSON Array of numeric values</returns>
    /// <exception cref="OverflowException"></exception>
    /// <exception cref="FormatException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public JToken? Execute(JToken? parameters)
    {
        if(parameters is JArray array)
        {
            if (array.Count() < 3) throw new ArgumentException("Not enough content: " + array);
            var input = array?.ToObject<Complex[]>() ?? Array.Empty<Complex>();
            using var pinIn = new PinnedArray<Complex>(input);
            using var pinOut = new FftwArrayDouble(GetRealBufferSize(pinIn.GetSize()));

            //DC needs no scaling to RMS
            var scale = 1 / Math.Sqrt(2);//AC to RMS
            for (int i = 1; i < input.Length; i++)
            {
                input[i] *= scale;
            }

            DFT.IFFT(pinIn, pinOut);

            var result = new JArray();
            for (int i = 0, len = pinOut.Length; i < len; i++)
            {
                result.Add(JToken.FromObject(pinOut[i]));
            }

            return result;
        }
        return null;
    }

    public string GetID()
    {
        return "IFFT";
    }
}
