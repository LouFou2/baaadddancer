using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI; //** only needed for debug viz

public class AudioFrequalizer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    private ClockCounter clockCounter;

    public static float[] samplesL = new float[512];
    public static float[] samplesR = new float[512];

    public static float[] freqBand8L = new float[8];
    public static float[] freqBand8R = new float[8];

    public static float[] freqBand5L = new float[5];
    public static float[] freqBand5R = new float[5];

    public static float[] bandBufferL = new float[8];
    public static float[] bandBufferR = new float[8];
    static float[] bufferDecreaseL = new float[8];
    static float[] bufferDecreaseR = new float[8];

    public float[] bandPusher5L = new float[5];
    public float[] bandPusher5R = new float[5];

    /*// THis is to calculate an average value for each frequency band 5: *** REMOVE: don't need averaging
    public static float[] freqBand5Sum = new float[5];
    public static int sampleCount = 0;
    public static float[] averagedFreqBand5 = new float[5];*/

    // Debugging Visualisers ** REMOVE LATER
    [SerializeField] private Image[] freq5BandVizL;    // Array to hold the cubes for each frequency band
    [SerializeField] private Image beatViz;
    private float beatLightAmount = 0;
    private float beatDuration;
    private float beatTime = 0;

    //*** FOR DEBUGGING
    //float maxFreqAmountL = 0;

    private void OnEnable()
    {
        ClockCounter.On_Q_Beat_Trigger += On_Q_BeatHandler; // Subscribe to the beat trigger event
        ClockCounter.On_Beat_Trigger += OnBeatHandler;
    }
    private void OnDisable()
    {
        ClockCounter.On_Q_Beat_Trigger -= On_Q_BeatHandler; // Subscribe to the beat trigger event
        ClockCounter.On_Beat_Trigger -= OnBeatHandler;
    }
    void Start()
    {
        clockCounter = FindObjectOfType<ClockCounter>(); // needed to get beat duration
    }

    /*void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        BandBuffer();
        Make5FrequencyBands();

        PulseBeatLight();

    }*/
    void On_Q_BeatHandler()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        BandBuffer();
        Make5FrequencyBands();

        PulseBeatLight();

        DebugVizualize5BandFreqs(); 
    }
    void OnBeatHandler()
    {
        beatLightAmount = 1;
        beatDuration = clockCounter.GetBeatInterval();
        beatTime = 0;
    }
    void PulseBeatLight()
    {
        beatTime += Time.deltaTime;
        beatLightAmount -= Mathf.Lerp(0, beatDuration, beatTime);
    }

    void GetSpectrumAudioSource()
    {
        audioSource.GetSpectrumData(samplesL, 0, FFTWindow.Blackman);
        audioSource.GetSpectrumData(samplesR, 1, FFTWindow.Blackman);
    }

    void MakeFrequencyBands()
    {
        int count = 0;

        for (int i = 0; i < 8; i++)
        {
            float averageL = 0;
            float averageR = 0;

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
                averageL += samplesL[count] * (count + 1);
                averageR += samplesR[count] * (count + 1);
                count++;
            }

            averageL /= count;
            averageR /= count;

            freqBand8L[i] = averageL;
            freqBand8R[i] = averageR;
        }
    }

    void BandBuffer()
    {
        for (int i = 0; i < 8; i++)
        {
            if (freqBand8L[i] > bandBufferL[i])
            {
                bandBufferL[i] = freqBand8L[i];
                bufferDecreaseL[i] = 0.005f;
            }
            if (freqBand8L[i] < bandBufferL[i])
            {
                bandBufferL[i] -= bufferDecreaseL[i];
                bufferDecreaseL[i] *= 1.2f;
            }

            if (freqBand8R[i] > bandBufferR[i])
            {
                bandBufferR[i] = freqBand8R[i];
                bufferDecreaseR[i] = 0.005f;
            }
            if (freqBand8R[i] < bandBufferR[i])
            {
                bandBufferR[i] -= bufferDecreaseR[i];
                bufferDecreaseR[i] *= 1.2f;
            }
        }
    }

    void Make5FrequencyBands()
    {
        // Combine pairs of bands to get a 5-band average
        freqBand5L[0] = (freqBand8L[0] + freqBand8L[1]) / 2f;
        freqBand5L[1] = (freqBand8L[2] + freqBand8L[3]) / 2f;
        freqBand5L[2] = freqBand8L[4];  // Keep this as a single band
        freqBand5L[3] = freqBand8L[5];  // Keep this as a single band
        freqBand5L[4] = (freqBand8L[6] + freqBand8L[7]) / 2f;

        freqBand5R[0] = (freqBand8R[0] + freqBand8R[1]) / 2f;
        freqBand5R[1] = (freqBand8R[2] + freqBand8R[3]) / 2f;
        freqBand5R[2] = freqBand8R[4];  // Keep this as a single band
        freqBand5R[3] = freqBand8R[5];  // Keep this as a single band
        freqBand5R[4] = (freqBand8R[6] + freqBand8R[7]) / 2f;

        for (int i = 0; i < 5; i++)
        {
            /*// **REMOVE LATER, DEBUGGING:
            if (freqBand5[i] > maxFreqAmount)
            {
                maxFreqAmount = freqBand5[i];
                Debug.Log("Max Freq Amount: " + maxFreqAmount);
            }
            // ****/

            freqBand5L[i] *= bandPusher5L[i];
            freqBand5R[i] *= bandPusher5R[i];

            /*// Accumulate values for averaging *** REMOVE: don't need averaging, as I already only calculate frequency value every q-Beat
            freqBand5Sum[i] += freqBand5[i];*/
        }
        //sampleCount++; *** REMOVE: don't need averaging
    }
    void DebugVizualize5BandFreqs() // *** CAN REMOVE IF NOT USING DEBUG VISUALISATION
    {
        /*if (sampleCount == 0) return;

        // Calculate the average *** REMOVE: don't need averaging, as I already only calculate frequency value every q-Beat
        for (int i = 0; i < 5; i++)
        {
            averagedFreqBand5[i] = freqBand5Sum[i] / sampleCount;
        }

        // Reset accumulators
        freqBand5Sum = new float[5];
        sampleCount = 0;*/

        //*** REMOVE LATER: VIZUALIZERS
        // Update the cubes based on the frequency data every frame
        if (freq5BandVizL.Length != 5)
        {
            return;
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                float height = freqBand5L[i] * 10;  // Scale to make it more visible
                freq5BandVizL[i].rectTransform.localScale = new Vector3(freq5BandVizL[i].rectTransform.localScale.x, height, freq5BandVizL[i].rectTransform.localScale.z);  // Adjust height (y-axis scale)
            }
        }
        Color color = beatViz.color;
        color.a = beatLightAmount;
        beatViz.color = color;
    }
}
