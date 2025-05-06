using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreqMover : MonoBehaviour // a script to move objects between two positions using frequency analyzer
{
    [SerializeField] private ClockCounter clockCounter;
    [SerializeField] private AudioFrequalizer audioFrequalizer;
    private float[] freqValues = new float[5];
    private float[] bandPushers = new float[5];
    [SerializeField] private GameObject[] moveObjectsL; // should be 5, but should do left AND right
    [SerializeField] private float[] moveObjectRangeL = new float[5];
    private bool[] canSwitchDirL = new bool[5];
    [SerializeField] private int resetCount = 2; // how many beats befor we reset the MaxFrequency
    private int beatCount = 0;

    private float maxFrequency;

    [SerializeField] private bool[] directionL = new bool[5]; // if true, it is positive direction, false is negative direction

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
        clockCounter = FindObjectOfType<ClockCounter>();

        for (int i = 0; i < 5; i++)
        {
            bandPushers[i] = audioFrequalizer.bandPusher5[i];
            canSwitchDirL[i] = false;
        }
    }

    private void On_Q_BeatHandler()
    {
        for (int i = 0; i < freqValues.Length; i++)
        {
            freqValues[i] = AudioFrequalizer.freqBand5[i] / bandPushers[i]; // the bandpushers multiplies the frequency values so we need the original value
            if (freqValues[i] > maxFrequency)
            {
                maxFrequency = freqValues[i]; //should be between 0-1 now
                Debug.Log(maxFrequency);
            }
        }
    }
    private void OnBeatHandler()
    {
        beatCount++;
        if (beatCount == resetCount) // we need to reset the maxFreq every little bit so it doesnt stay stuck on an unreachable max
        {
            maxFrequency = 0;
            beatCount = 0;
            Debug.Log("reset");
        }
    }

    private void Update()
    {
        for (int i = 0; i < freqValues.Length; i++)
        {
            if (freqValues[i] >= maxFrequency) // although it can only be ==, never more (i think)
            {
                canSwitchDirL[i] = true;
            }
            if (freqValues[i] < maxFrequency && canSwitchDirL[i])
            {
                directionL[i] = !directionL[i];
                canSwitchDirL[i] = false;
            }

            if (directionL[i])
            {
                float lerpedY = Mathf.Lerp(0, moveObjectRangeL[i], freqValues[i]);
                moveObjectsL[i].transform.position = new Vector3(moveObjectsL[i].transform.position.x, lerpedY, moveObjectsL[i].transform.position.z);
            }
            if (!directionL[i])
            {
                float lerpedY = Mathf.Lerp(0, -moveObjectRangeL[i], freqValues[i]); // note: its lerping to the negative
                moveObjectsL[i].transform.position = new Vector3(moveObjectsL[i].transform.position.x, lerpedY, moveObjectsL[i].transform.position.z);
            }
        }
    }
}
