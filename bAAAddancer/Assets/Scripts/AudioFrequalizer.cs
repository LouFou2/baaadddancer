using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFrequalizer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    public static float[] samples = new float[512];
    public static float[] freqBand8 = new float[8];
    public static float[] freqBand5 = new float[5];
    public static float[] bandBuffer = new float[8];
    static float[] bufferDecrease = new float[8];

    public float hertz;

    /*void Start()
    {
        hertz = audioSource.clip.frequency;
    }*/

    void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        BandBuffer();
        Make5FrequencyBands();
    }

    void GetSpectrumAudioSource()
    {
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Blackman);
    }

    void MakeFrequencyBands()
    {
        int count = 0;

        for (int i = 0; i < 8; i++)
        {
            float average = 0;

            // the calculation below sets up 8 "frequency bands". It works exponentially, rather than dividing all frequencies equally:
            // e.g:
            // (i=0) -> 2 to the pow of 0 = 1. 1 * 2 = 2
            // (i=1) -> 2 to pow of 1 = 2. *2 = 4
            // (i=2) -> 2 to pow of 2 = 4. *2 = 8
            // .. i=3 -> 16, i=4 -> 32, i=5 -> 64... 128, 256, 512...

            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            // we add 2 here, because in the last iteration, when the bands are added up (2+4+8+16...etc. it comes to 510, and we want to use all 512)
            if (i == 7) // this is the last iteration of i
            {
                sampleCount += 2;
            }

            for (int j = 0; j < sampleCount; j++)
            {
                average += samples[count] * (count + 1);
                count++;
            }

            average /= count;

            freqBand8[i] = average * 10;
        }
    }

    void BandBuffer()
    {
        for (int i = 0; i < 8; i++)
        {
            if (freqBand8[i] > bandBuffer[i])
            {
                bandBuffer[i] = freqBand8[i];
                bufferDecrease[i] = 0.005f;
            }
            if (freqBand8[i] < bandBuffer[i])
            {
                bandBuffer[i] -= bufferDecrease[i];
                bufferDecrease[i] *= 1.2f;
            }
        }
    }

    void Make5FrequencyBands()
    {
        // Combine pairs of bands to get a 5-band average
        freqBand5[0] = (freqBand8[0] + freqBand8[1]) / 2f;
        freqBand5[1] = (freqBand8[2] + freqBand8[3]) / 2f;
        freqBand5[2] = freqBand8[4];  // Keep this as a single band
        freqBand5[3] = freqBand8[5];  // Keep this as a single band
        freqBand5[4] = (freqBand8[6] + freqBand8[7]) / 2f;
    }
}
