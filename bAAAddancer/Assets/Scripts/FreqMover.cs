using System;
using UnityEngine;

public class FreqMover : MonoBehaviour // a script to move objects between two positions using frequency analyzer
{
    [SerializeField] private AudioFrequalizer audioFrequalizer;
    private float[] freqValuesL = new float[5];
    private float[] freqValuesR = new float[5];
    
    [SerializeField] private GameObject[] moveObjectsL; // should be 5, but should do left AND right
    [SerializeField] private GameObject[] moveObjectsR;

    [SerializeField] private float[] moveObjectRange = new float[5]; // instead of this, we will use some given Vector3 positions

    [SerializeField] private int resetCount = 2; // how many beats befor we set new targets
    private int beatCount = 0;

    private float[] maxFrequencyL = new float[5];
    private float[] maxFrequencyR = new float[5];

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
    private Vector3[] moveObjectInitialPosR = new Vector3[5]; 

    private Vector3[] moveObjectTargetPosA_L = new Vector3[5]; // we will set two target positions to lerp between
    private Vector3[] moveObjectTargetPosB_L = new Vector3[5]; 
    private Vector3[] moveObjectTargetPosA_R = new Vector3[5]; // for each side
    private Vector3[] moveObjectTargetPosB_R = new Vector3[5];

    [Serializable]
    public class ObjectLimits
    {
        public float xMin = -1;
        public float xMax = 1;
        public float yMin = -1;
        public float yMax = 1;
        public float zMin = -1;
        public float zMax = 1;
    }

    [SerializeField] private ObjectLimits[] objectMovementLimitL = new ObjectLimits[5];
    [SerializeField] private ObjectLimits[] objectMovementLimitR = new ObjectLimits[5];

    //*** REMOVE LATER: just for debugging
    private GameObject[] leftMarkersA = new GameObject[5];
    private GameObject[] leftMarkersB = new GameObject[5];
    private GameObject[] rightMarkersA = new GameObject[5];
    private GameObject[] rightMarkersB = new GameObject[5];

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
        for (int i = 0; i < 5; i++)
        {
            // storing the initial default positions
            moveObjectInitialPosL[i] = moveObjectsL[i].transform.position;
            moveObjectInitialPosR[i] = moveObjectsR[i].transform.position;

        }
        SetTargetPositionPairs();
    }
    void SetTargetPositionPairs()
    {
        for (int i = 0; i < 5; i++) // for each object, we set a main direction, with randomised secondary values
        {
            int mainDirectionL = UnityEngine.Random.Range(0,3);
            int posOrNegL = UnityEngine.Random.Range(0, 2);
            float xDirL = 0;
            float yDirL = 0;
            float zDirL = 0;
            switch (mainDirectionL)
            {
                case 0: // x is main direction
                    xDirL = (posOrNegL == 0) ? objectMovementLimitL[i].xMin : objectMovementLimitL[i].xMax;
                    yDirL = UnityEngine.Random.Range(objectMovementLimitL[i].yMin, objectMovementLimitL[i].yMax);
                    zDirL = UnityEngine.Random.Range(objectMovementLimitL[i].zMin, objectMovementLimitL[i].zMax);
                    break;
                case 1: // y is main direction
                    xDirL = UnityEngine.Random.Range(objectMovementLimitL[i].xMin, objectMovementLimitL[i].xMax);
                    yDirL = (posOrNegL == 0) ? objectMovementLimitL[i].yMin : objectMovementLimitL[i].yMax;
                    zDirL = UnityEngine.Random.Range(objectMovementLimitL[i].zMin, objectMovementLimitL[i].zMax);
                    break;
                case 2: // z is main direction
                    xDirL = UnityEngine.Random.Range(objectMovementLimitL[i].xMin, objectMovementLimitL[i].xMax);
                    yDirL = UnityEngine.Random.Range(objectMovementLimitL[i].yMin, objectMovementLimitL[i].yMax);
                    zDirL = (posOrNegL == 0) ? objectMovementLimitL[i].zMin : objectMovementLimitL[i].zMax;
                    break;
                default:
                    mainDirectionL = 0;
                    break;
            }

            int mainDirectionR = UnityEngine.Random.Range(0, 3);
            int posOrNegR = UnityEngine.Random.Range(0, 2);
            float xDirR = 0;
            float yDirR = 0;
            float zDirR = 0;
            switch (mainDirectionR)
            {
                case 0: // x is main direction
                    xDirR = (posOrNegR == 0) ? objectMovementLimitR[i].xMin : objectMovementLimitR[i].xMax;
                    yDirR = UnityEngine.Random.Range(objectMovementLimitR[i].yMin, objectMovementLimitR[i].yMax);
                    zDirR = UnityEngine.Random.Range(objectMovementLimitR[i].zMin, objectMovementLimitR[i].zMax);
                    break;
                case 1: // y is main direction
                    xDirR = UnityEngine.Random.Range(objectMovementLimitR[i].xMin, objectMovementLimitR[i].xMax);
                    yDirR = (posOrNegL == 0) ? objectMovementLimitR[i].yMin : objectMovementLimitR[i].yMax;
                    zDirR = UnityEngine.Random.Range(objectMovementLimitR[i].zMin, objectMovementLimitR[i].zMax);
                    break;
                case 2: // z is main direction
                    xDirR = UnityEngine.Random.Range(objectMovementLimitR[i].xMin, objectMovementLimitR[i].xMax);
                    yDirR = UnityEngine.Random.Range(objectMovementLimitR[i].yMin, objectMovementLimitR[i].yMax);
                    zDirR = (posOrNegL == 0) ? objectMovementLimitR[i].zMin : objectMovementLimitR[i].zMax;
                    break;
                default:
                    mainDirectionR = 0;
                    break;
            }

            Vector3 dirL = new Vector3(xDirL, yDirL, zDirL).normalized;
            Vector3 dirR = new Vector3(xDirR, yDirR, zDirR).normalized;

            moveObjectTargetPosA_L[i] = moveObjectInitialPosL[i] + (dirL * moveObjectRange[i]);
            moveObjectTargetPosA_R[i] = moveObjectInitialPosR[i] + (dirR * moveObjectRange[i]);
        }

        //***REMOVE LATER: debugging
        //ShowTargetMarkers(3);
        //***
    }
    
    private void On_Q_BeatHandler()
    {
        float[] lerpValueL = new float[5]; // using this makes quite a difference
        float[] lerpValueR = new float[5];

        for (int i = 0; i < freqValuesL.Length; i++)
        {
            freqValuesL[i] = AudioFrequalizer.freqBand5L[i];

            if (freqValuesL[i] > maxFrequencyL[i])
            {
                maxFrequencyL[i] = freqValuesL[i];
            }

            lerpValueL[i] = Mathf.InverseLerp(0, maxFrequencyL[i], freqValuesL[i]); // this uses a 0-1 range using the max frequency as 1

        }
        for (int i = 0; i < freqValuesR.Length; i++)
        {
            freqValuesR[i] = AudioFrequalizer.freqBand5R[i];

            if (freqValuesR[i] > maxFrequencyR[i])
            {
                maxFrequencyR[i] = freqValuesR[i];
            }

            lerpValueR[i] = Mathf.InverseLerp(0, maxFrequencyR[i], freqValuesR[i]);

        }

        // Lerping:
        for (int i = 0; i < freqValuesL.Length; i++)
        {
            moveObjectsL[i].transform.position = Vector3.Lerp(moveObjectInitialPosL[i], moveObjectTargetPosA_L[i], lerpValueL[i]);
        }
        for (int i = 0; i < freqValuesR.Length; i++)
        {
            moveObjectsR[i].transform.position = Vector3.Lerp(moveObjectInitialPosR[i], moveObjectTargetPosA_R[i], lerpValueR[i]);
        }
    }
    private void OnBeatHandler()
    {
        beatCount++;
        if (beatCount == resetCount) // we need to reset the loop duration for some rythm variety
        {
            resetCount = UnityEngine.Random.Range(1, 6); // every reset we randomise the  move loop duration
            SetTargetPositionPairs();
            beatCount = 0;
        }
    }

    // *** REMOVE LATER: just for debugging, can remove
    void ShowTargetMarkers(int index)
    {
        if (leftMarkersA[index] != null) Destroy(leftMarkersA[index]);
        if (leftMarkersB[index] != null) Destroy(leftMarkersB[index]);
        if (rightMarkersA[index] != null) Destroy(rightMarkersA[index]);
        if (rightMarkersB[index] != null) Destroy(rightMarkersB[index]);

        // Instantiate or use primitive cube
        leftMarkersA[index] = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftMarkersB[index] = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightMarkersA[index] = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightMarkersB[index] = GameObject.CreatePrimitive(PrimitiveType.Cube);

        // Resize to small cubes
        leftMarkersA[index].transform.localScale = Vector3.one * 0.1f;
        leftMarkersB[index].transform.localScale = Vector3.one * 0.1f;
        rightMarkersA[index].transform.localScale = Vector3.one * 0.1f;
        rightMarkersB[index].transform.localScale = Vector3.one * 0.1f;

        // Position
        leftMarkersA[index].transform.position = moveObjectTargetPosA_L[index];
        leftMarkersB[index].transform.position = moveObjectTargetPosB_L[index];
        rightMarkersA[index].transform.position = moveObjectTargetPosA_R[index];
        rightMarkersB[index].transform.position = moveObjectTargetPosB_R[index];

        // Colors
        leftMarkersA[index].GetComponent<Renderer>().material.color = Color.blue;
        leftMarkersB[index].GetComponent<Renderer>().material.color = Color.cyan;
        rightMarkersA[index].GetComponent<Renderer>().material.color = Color.red;
        rightMarkersB[index].GetComponent<Renderer>().material.color = Color.magenta;

        /*for (int i = 0; i < 5; i++)
        {
            if (leftMarkersA[i] != null) Destroy(leftMarkersA[i]);
            if (leftMarkersB[i] != null) Destroy(leftMarkersB[i]);
            if (rightMarkersA[i] != null) Destroy(rightMarkersA[i]);
            if (rightMarkersB[i] != null) Destroy(rightMarkersB[i]);

            // Instantiate or use primitive cube
            leftMarkersA[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftMarkersB[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightMarkersA[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightMarkersB[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);

            // Resize to small cubes
            leftMarkersA[i].transform.localScale = Vector3.one * 0.1f;
            leftMarkersB[i].transform.localScale = Vector3.one * 0.1f;
            rightMarkersA[i].transform.localScale = Vector3.one * 0.1f;
            rightMarkersB[i].transform.localScale = Vector3.one * 0.1f;

            // Position
            leftMarkersA[i].transform.position = moveObjectTargetPosA_L[i];
            leftMarkersB[i].transform.position = moveObjectTargetPosB_L[i];
            rightMarkersA[i].transform.position = moveObjectTargetPosA_R[i];
            rightMarkersB[i].transform.position = moveObjectTargetPosB_R[i];

            // Colors
            leftMarkersA[i].GetComponent<Renderer>().material.color = Color.blue;
            leftMarkersB[i].GetComponent<Renderer>().material.color = Color.cyan;
            rightMarkersA[i].GetComponent<Renderer>().material.color = Color.red;
            rightMarkersB[i].GetComponent<Renderer>().material.color = Color.magenta;
        }*/
    }

}
