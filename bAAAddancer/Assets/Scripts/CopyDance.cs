using DG.Tweening;
using System;
using UnityEngine;

public class CopyDance : MonoBehaviour
{
    [Serializable]
    public class CopyTransforms
    {
        public GameObject copyObject;
        public RoundsRecData recDataObjSequencer;
        public RecordingData recordingDataSO;
        public Vector3[] targetRecordedPositions;
        public Vector3[] offset;
    }
    [SerializeField] private CopyTransforms[] objectsAndMoveData;
    
    private ClockCounter clockCounter;
    private PlayerControls playerControls;

    public bool charLeftScreen;
    public bool charCenterScreen;
    public bool charRightScreen;

    int roundSwitcherIndex = -1;
    [SerializeField] bool updatingRoundSequence = false;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }
    private void OnEnable()
    {
        playerControls.Enable();
        ClockCounter.On_Q_BeatTrigger += On_Q_BeatHandler; // Subscribe to the beat trigger event
    }
    private void OnDisable()
    {
        playerControls.Disable();
        ClockCounter.On_Q_BeatTrigger -= On_Q_BeatHandler; // Subscribe to the beat trigger event
    }

    void Start()
    {
        clockCounter = FindObjectOfType<ClockCounter>();
        if (clockCounter == null ) 
            Debug.LogError("no clock counter in scene");

        roundSwitcherIndex = 0;

        // Adjust Vector3 positions according to offsets
        for(int i = 0; i < objectsAndMoveData.Length; i++) // for each object referenced in the script
        {
            objectsAndMoveData[i].recordingDataSO = objectsAndMoveData[i].recDataObjSequencer.currentRoundRecData;
            objectsAndMoveData[i].targetRecordedPositions = new Vector3[objectsAndMoveData[i].recordingDataSO.recordedPositions.Length];
            objectsAndMoveData[i].offset = new Vector3[objectsAndMoveData[i].recordingDataSO.recordedPositions.Length];

            for (int j = 0; j < objectsAndMoveData[i].targetRecordedPositions.Length; j++) // for each recorded Vector3 position
            {
                // Calculate the offset between the control character objects and the dancer character objects:
                float offsetX = objectsAndMoveData[i].copyObject.transform.position.x - objectsAndMoveData[i].recordingDataSO.initialPositions[j].x;
                float offsetY = objectsAndMoveData[i].copyObject.transform.position.y - objectsAndMoveData[i].recordingDataSO.initialPositions[j].y;
                float offsetZ = objectsAndMoveData[i].copyObject.transform.position.z - objectsAndMoveData[i].recordingDataSO.initialPositions[j].z;

                objectsAndMoveData[i].offset[j] = new Vector3(offsetX, offsetY, offsetZ); //store the offset so we can use it in UpdateDanceSequence

                // introduce "noise" to offsets?

                float adjustedX = objectsAndMoveData[i].recordingDataSO.recordedPositions[j].x + offsetX;
                float adjustedY = objectsAndMoveData[i].recordingDataSO.recordedPositions[j].y + offsetY; ;
                float adjustedZ = objectsAndMoveData[i].recordingDataSO.recordedPositions[j].z + offsetZ; ;
                objectsAndMoveData[i].targetRecordedPositions[j] = new Vector3(adjustedX, adjustedY, adjustedZ);
            }
        }
        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !updatingRoundSequence) //replace this with correct input logic
        {
            UpdateDanceSequence();
        }
            
    }
    private void UpdateDanceSequence() 
    {
        updatingRoundSequence = true;
        roundSwitcherIndex += 1;
        if (roundSwitcherIndex > 3) roundSwitcherIndex = 0; // loop 

        // Adjust Vector3 positions according to offsets
        for (int i = 0; i < objectsAndMoveData.Length; i++) // for each object referenced in the script
        {
            objectsAndMoveData[i].recordingDataSO = objectsAndMoveData[i].recDataObjSequencer.recordingDataOfRounds[roundSwitcherIndex];
            objectsAndMoveData[i].targetRecordedPositions = new Vector3[objectsAndMoveData[i].recordingDataSO.recordedPositions.Length];

            for (int j = 0; j < objectsAndMoveData[i].targetRecordedPositions.Length; j++) // for each recorded Vector3 position
            {
                /*// Calculate the offset between the control character objects and the dancer character objects:
                float offsetX = objectsAndMoveData[i].copyObject.transform.position.x - objectsAndMoveData[i].recordingDataSO.initialPositions[j].x;
                float offsetY = objectsAndMoveData[i].copyObject.transform.position.y - objectsAndMoveData[i].recordingDataSO.initialPositions[j].y;
                float offsetZ = objectsAndMoveData[i].copyObject.transform.position.z - objectsAndMoveData[i].recordingDataSO.initialPositions[j].z;*/

                // introduce "noise" to offsets?

                float adjustedX = objectsAndMoveData[i].recordingDataSO.recordedPositions[j].x + objectsAndMoveData[i].offset[j].x; //using offsets stored in Start
                float adjustedY = objectsAndMoveData[i].recordingDataSO.recordedPositions[j].y + objectsAndMoveData[i].offset[j].y;
                float adjustedZ = objectsAndMoveData[i].recordingDataSO.recordedPositions[j].z + objectsAndMoveData[i].offset[j].z;
                objectsAndMoveData[i].targetRecordedPositions[j] = new Vector3(adjustedX, adjustedY, adjustedZ);

                //objectsAndMoveData[i].targetRecordedPositions[j] = objectsAndMoveData[i].recordingDataSO.recordedPositions[j];
            }
        }
        updatingRoundSequence = false;
    }

    private void On_Q_BeatHandler() 
    {
        GameObject copyCharacter = gameObject;
        CharacterProfile characterProfile = copyCharacter.GetComponent<CharacterProfile>();

        for (int objectsIndex = 0; objectsIndex < objectsAndMoveData.Length; objectsIndex++)
        {
            GameObject copyingObject = objectsAndMoveData[objectsIndex].copyObject;
            Vector3[] targetRecordedPositions = objectsAndMoveData[objectsIndex].targetRecordedPositions;

            int currentBeatIndex = clockCounter.GetCurrent_Q_Beat(); //the indexes of the positions corresponds to the beats (the beats are used to record them)
            int targetBeatIndex = currentBeatIndex + 1;

            // make the beat + target count loop:
            if (targetBeatIndex > objectsAndMoveData[objectsIndex].recordingDataSO.recordedPositions.Length - 1) targetBeatIndex = 0;
            if (targetBeatIndex < 0) targetBeatIndex = 0;
            float tweenDuration = clockCounter.Get_Q_BeatInterval();

            //OLD FRAME DROP LOGIC:
            /* if (characterProfile.characterDataSO.infectionLevel > 0) // if the character is infected
            {
                // call infection behaviours
                int frameRateDivider = 1; // change how much to divide the frame rate
                int glitchedBeatIndex = 0;
                if (currentBeatIndex % frameRateDivider == 0) glitchedBeatIndex = currentBeatIndex;
                copyingObject.transform.position = targetRecordedPositions[glitchedBeatIndex];

            }
            else
            { // Tween the object's position to the next recorded position
                copyingObject.transform.DOMove(targetRecordedPositions[targetBeatIndex], tweenDuration).SetEase(Ease.Linear);
            } */
            
            copyingObject.transform.DOMove(targetRecordedPositions[targetBeatIndex], tweenDuration).SetEase(Ease.Linear);
        }

    }
}
