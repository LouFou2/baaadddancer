using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI; //** only needed for debug viz

public class AudioFrequalizer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    public static float[] samples = new float[512];
    public static float[] freqBand8 = new float[8];
    public static float[] freqBand5 = new float[5];
    public static float[] bandBuffer = new float[8];
    static float[] bufferDecrease = new float[8];

    [SerializeField] private float[] bandPusher5 = new float[5];

    // THis is to calculate an average value for each frequency band 5:
    public static float[] freqBand5Sum = new float[5];
    public static int sampleCount = 0;
    public static float[] averagedFreqBand5 = new float[5];

    // Debugging Visualisers ** REMOVE LATER
    [SerializeField] private Image[] freq5BandViz;    // Array to hold the cubes for each frequency band

    private void OnEnable()
    {
        ClockCounter.On_Q_Beat_Trigger += On_Q_BeatHandler; // Subscribe to the beat trigger event
    }
    private void OnDisable()
    {
        ClockCounter.On_Q_Beat_Trigger -= On_Q_BeatHandler; // Subscribe to the beat trigger event
    }

    void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        BandBuffer();
        Make5FrequencyBands();
        
    }
    void On_Q_BeatHandler()
    {
        Average5BandFreqs();
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

        for (int i = 0; i < freqBand5.Length; i++)
        {
            freqBand5[i] *= bandPusher5[i];

            // Accumulate values for averaging
            freqBand5Sum[i] += freqBand5[i];
        }
        sampleCount++;
    }
    void Average5BandFreqs()
    {
        if (sampleCount == 0) return;

        // Calculate the average
        for (int i = 0; i < 5; i++)
        {
            averagedFreqBand5[i] = freqBand5Sum[i] / sampleCount;
        }

        // Reset accumulators
        freqBand5Sum = new float[5];
        sampleCount = 0;

        //*** REMOVE LATER: VIZUALIZERS
        // Update the cubes based on the frequency data every frame
        if (freq5BandViz.Length == 5)
        {
            for (int i = 0; i < 5; i++)
            {
                float height = averagedFreqBand5[i] * 10;  // Scale to make it more visible
                freq5BandViz[i].rectTransform.localScale = new Vector3(freq5BandViz[i].rectTransform.localScale.x, height, freq5BandViz[i].rectTransform.localScale.z);  // Adjust height (y-axis scale)
            }
        }
        else
        {
            return;
        }
        
    }
}
