using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreqMover : MonoBehaviour // a script to move objects between two positions using frequency analyzer
{
    //[SerializeField] private ClockCounter clockCounter; //***REMOVE: NOT USING THIS?
    [SerializeField] private AudioFrequalizer audioFrequalizer;
    private float[] freqValuesL = new float[5];
    private float[] freqValuesR = new float[5];
    private float[] bandPushersL = new float[5];
    private float[] bandPushersR = new float[5];

    [SerializeField] private GameObject[] moveObjectsL; // should be 5, but should do left AND right
    [SerializeField] private GameObject[] moveObjectsR;

    [SerializeField] private float[] moveObjectRange = new float[5]; // instead of this, we will use some given Vector3 positions

    [SerializeField] private int resetCount = 2; // how many beats befor we reset the MaxFrequency
    private int beatCount = 0;

    /*private float[] maxFrequencyL = new float[5]; // ***REMOVE? don't really need this, was using it to store the peaks
    private float[] maxFrequencyR = new float[5];*/

    /* We need cool logic to set up target positions for moveObjects to lerp between
     * OPTION A - we can check the recorded positions and find targets that are:
     * 1. relatively far from the default/intitial position (iterate through the array)
     * 2. then find another position that is the furthest from this position (iterate through the array again)
     * OPTION B - we can semi-randomly set target positions that are: 
     * 1. relatively far from the initial position of the object
     * 2. another semi-random position that is far-ish from the first target
    */
    // trying Option B first:
    private Vector3[] moveObjectInitialPosL = new Vector3[5]; // get a reference to the starting positions
    private Vector3[] moveObjectInitialPosR = new Vector3[5]; // *** not sure we actually need this

    private Vector3[] moveObjectTargetPosA_L = new Vector3[5]; // we will set two target positions to lerp between
    private Vector3[] moveObjectTargetPosB_L = new Vector3[5]; 
    private Vector3[] moveObjectTargetPosA_R = new Vector3[5]; // for each side
    private Vector3[] moveObjectTargetPosB_R = new Vector3[5]; 

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
        //clockCounter = FindObjectOfType<ClockCounter>(); //***REMOVE: NOT USING THIS?

        for (int i = 0; i < 5; i++)
        {
            bandPushersL[i] = audioFrequalizer.bandPusher5L[i];
            bandPushersR[i] = audioFrequalizer.bandPusher5R[i];

            // storing the initial default positions
            moveObjectInitialPosL[i] = moveObjectsL[i].transform.position;
            moveObjectInitialPosR[i] = moveObjectsR[i].transform.position;
            
        }
        SetTargetPositionPairs();
    }
    void SetTargetPositionPairs()
    {
        for (int i = 0; i < 5; i++)
        {
            // Generate A direction
            Vector3 dirA_L = Random.onUnitSphere;
            Vector3 dirA_R = Random.onUnitSphere;

            // Generate B direction that is roughly opposite to A
            Vector3 dirB_L;
            do
            {
                dirB_L = Random.onUnitSphere;
            } while (Vector3.Dot(dirA_L, dirB_L) > -0.8f); // Adjust threshold as needed

            Vector3 dirB_R;
            do
            {
                dirB_R = Random.onUnitSphere;
            } while (Vector3.Dot(dirA_R, dirB_R) > -0.8f);

            // Assign target positions
            moveObjectTargetPosA_L[i] = moveObjectInitialPosL[i] + dirA_L * moveObjectRange[i];
            moveObjectTargetPosB_L[i] = moveObjectInitialPosL[i] + dirB_L * moveObjectRange[i];
            moveObjectTargetPosA_R[i] = moveObjectInitialPosR[i] + dirA_R * moveObjectRange[i];
            moveObjectTargetPosB_R[i] = moveObjectInitialPosR[i] + dirB_R * moveObjectRange[i];
        }
    }
    private void On_Q_BeatHandler()
    {
        for (int i = 0; i < freqValuesL.Length; i++)
        {
            freqValuesL[i] = AudioFrequalizer.freqBand5L[i] / bandPushersL[i]; // the bandpushers multiplies the frequency values so we need the original value

            /*if (freqValuesL[i] > maxFrequencyL[i])
            {
                maxFrequencyL[i] = freqValuesL[i]; //should be between 0-1 now
            }*/
        }
        for (int i = 0; i < freqValuesR.Length; i++)
        {
            freqValuesR[i] = AudioFrequalizer.freqBand5R[i] / bandPushersR[i]; // the bandpushers multiplies the frequency values so we need the original value

            /*if (freqValuesR[i] > maxFrequencyR[i])
            {
                maxFrequencyR[i] = freqValuesR[i]; //should be between 0-1 now
            }*/
        }

        // Lerping:
        for (int i = 0; i < freqValuesL.Length; i++)
        {
            moveObjectsL[i].transform.position = Vector3.Lerp(moveObjectTargetPosA_L[i], moveObjectTargetPosB_L[i], freqValuesL[i]);
            
        }
        for (int i = 0; i < freqValuesR.Length; i++)
        {
            moveObjectsR[i].transform.position = Vector3.Lerp(moveObjectTargetPosA_R[i], moveObjectTargetPosB_R[i], freqValuesR[i]);
            
        }
    }
    private void OnBeatHandler()
    {
        beatCount++;
        if (beatCount == resetCount) // we need to reset the loop duration for some rythm variety
        {
            resetCount = Random.Range(1, 6); // every reset we randomise the  move loop duration
            SetTargetPositionPairs();
            beatCount = 0;
        }
    }

}
