using DG.Tweening;
using System;
using UnityEngine;

public class CopyDance : MonoBehaviour
{
    [SerializeField] private bool raveDemonReveal = false;
    [Serializable]
    public class CopyTransforms
    {
        public GameObject copyObject;
        public GameObject copyDemonObject;
        public bool demonTransitioning = false;
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

    int currentRound = -1;
    [SerializeField] int roundSwitcherIndex = -1;
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
        updatingRoundSequence = false;

        raveDemonReveal = false;

        clockCounter = FindObjectOfType<ClockCounter>();
        if (clockCounter == null ) 
            Debug.LogError("no clock counter in scene");

        currentRound = GameManager.Instance.GetCurrentRound();
        roundSwitcherIndex = GameManager.Instance.GetCurrentRound();

        // Adjust Vector3 positions according to offsets
        for(int i = 0; i < objectsAndMoveData.Length; i++) // for each object referenced in the script
        {
            objectsAndMoveData[i].recordingDataSO = objectsAndMoveData[i].recDataObjSequencer.currentRoundRecData;
            
            //initialising the positions arrays
            objectsAndMoveData[i].targetRecordedPositions = new Vector3[objectsAndMoveData[i].recordingDataSO.recordedPositions.Length];
            objectsAndMoveData[i].offset = new Vector3[objectsAndMoveData[i].recordingDataSO.recordedPositions.Length];

            objectsAndMoveData[i].demonTransitioning = false;

            // for each recorded Vector3 position
            for (int j = objectsAndMoveData[i].targetRecordedPositions.Length - 1; j > -1; j--) // we count down so we can do 0 last*
            {
                if (i > 0) 
                {
                    // Calculate the offset between the control character objects and the dancer character objects:
                    float offsetX = objectsAndMoveData[i].copyObject.transform.localPosition.x - objectsAndMoveData[i].recordingDataSO.initialPositions[j].x;
                    float offsetY = objectsAndMoveData[i].copyObject.transform.localPosition.y - objectsAndMoveData[i].recordingDataSO.initialPositions[j].y;
                    float offsetZ = objectsAndMoveData[i].copyObject.transform.localPosition.z - objectsAndMoveData[i].recordingDataSO.initialPositions[j].z;

                    objectsAndMoveData[i].offset[j] = new Vector3(offsetX, offsetY, offsetZ); //store the offset so we can use it in UpdateDanceSequence

                    // introduce "noise" to offsets?

                    float adjustedX = objectsAndMoveData[i].recordingDataSO.recordedPositions[j].x + offsetX;
                    float adjustedY = objectsAndMoveData[i].recordingDataSO.recordedPositions[j].y + offsetY;
                    float adjustedZ = objectsAndMoveData[i].recordingDataSO.recordedPositions[j].z + offsetZ;
                    // * we also have to add root positions ("objectsAndMoveData[0]....") because the root doesn't function as a parent in the rig
                    objectsAndMoveData[i].targetRecordedPositions[j] = new Vector3(adjustedX, adjustedY, adjustedZ) + objectsAndMoveData[0].recordingDataSO.recordedPositions[j]; //*0 is root
                }
                //we add offset to each root object's positions array AFTER adding the recorded root positions to all the other objects' arrays
                //index 0 is the root control: the root object uses world space (transform.position)
                if (i == 0)
                {
                    objectsAndMoveData[0].targetRecordedPositions[0] // each recorded position of the root object 
                    = objectsAndMoveData[0].recordingDataSO.recordedPositions[0]
                    + objectsAndMoveData[0].copyObject.transform.position; // add the position of this characters root object as offset
                }
            }
        }
        /*
        //we add offset to each root object's positions array AFTER adding the recorded root positions to all the other objects' arrays
        for (int rootPositionIndex = 0; rootPositionIndex < objectsAndMoveData[0].targetRecordedPositions.Length; rootPositionIndex++)
        {
            objectsAndMoveData[0].targetRecordedPositions[rootPositionIndex] // each recorded position of the root object 
                = objectsAndMoveData[0].recordingDataSO.recordedPositions[rootPositionIndex] 
                + objectsAndMoveData[rootPositionIndex].copyObject.transform.position; // add the position of this characters root object as offset
        }*/
    }
    private void Update()
    {
        if (currentRound >= 1) 
        {
            if (playerControls.GenericInput.LBumper.triggered && !updatingRoundSequence)
            {
                UpdateDanceSequence(-1);
            }
            if (playerControls.GenericInput.RBumper.triggered && !updatingRoundSequence) 
            {
                UpdateDanceSequence(1);
            }
        }
    }
    private void UpdateDanceSequence(int posOrNegInt) 
    {
        updatingRoundSequence = true;
        roundSwitcherIndex += posOrNegInt;
        if (roundSwitcherIndex > currentRound) roundSwitcherIndex = 0; // loop, also limit switch index to amount of rounds
        if (roundSwitcherIndex < 0) roundSwitcherIndex = currentRound; // loop, also limit switch index to amount of rounds

        // Adjust Vector3 positions according to offsets
        for (int i = 0; i < objectsAndMoveData.Length; i++) // for each object referenced in the script
        {
            objectsAndMoveData[i].recordingDataSO = objectsAndMoveData[i].recDataObjSequencer.recordingDataOfRounds[roundSwitcherIndex];
            objectsAndMoveData[i].targetRecordedPositions = new Vector3[objectsAndMoveData[i].recordingDataSO.recordedPositions.Length];

            for (int j = objectsAndMoveData[i].targetRecordedPositions.Length - 1; j > -1; j--) // we count down so we can do 0 last*
            {
                if (i > 0)
                {
                    // introduce "noise" to offsets?
                    float adjustedX = objectsAndMoveData[i].recordingDataSO.recordedPositions[j].x + objectsAndMoveData[i].offset[j].x; //using offsets stored in Start
                    float adjustedY = objectsAndMoveData[i].recordingDataSO.recordedPositions[j].y + objectsAndMoveData[i].offset[j].y;
                    float adjustedZ = objectsAndMoveData[i].recordingDataSO.recordedPositions[j].z + objectsAndMoveData[i].offset[j].z;
                    objectsAndMoveData[i].targetRecordedPositions[j] = new Vector3(adjustedX, adjustedY, adjustedZ) + objectsAndMoveData[0].recordingDataSO.recordedPositions[j]; //*0 is root
                }
                if (i == 0)
                {
                    objectsAndMoveData[0].targetRecordedPositions[0] // each recorded position of the root object 
                        = objectsAndMoveData[0].recordingDataSO.recordedPositions[0]
                        + objectsAndMoveData[0].copyObject.transform.position; // add the position of this characters root object as offset
                }
            }
        }
/*
        for (int rootPositionIndex = 0; rootPositionIndex < objectsAndMoveData[0].targetRecordedPositions.Length; rootPositionIndex++)
        {
            objectsAndMoveData[0].targetRecordedPositions[rootPositionIndex] // each recorded position of the root object 
                = objectsAndMoveData[0].recordingDataSO.recordedPositions[rootPositionIndex]
                + objectsAndMoveData[rootPositionIndex].copyObject.transform.position; // add the position of this characters root object as offset
        }
*/
        updatingRoundSequence = false;
    }

    private void On_Q_BeatHandler() // ***TOCHECK: check here for extra offset added to char positions. The extra offset isn't on root control object, just the others
    {
        if (!raveDemonReveal)
        {
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

                if (objectsIndex == 0) // if its the Root Object, we use world space
                {
                    copyingObject.transform.DOMove(targetRecordedPositions[targetBeatIndex], tweenDuration).SetEase(Ease.Linear);
                }
                else // else its local space
                {
                    copyingObject.transform.DOLocalMove(targetRecordedPositions[targetBeatIndex], tweenDuration).SetEase(Ease.Linear);
                }
                
            }
        }
        else 
        {
            for (int objectsIndex = 0; objectsIndex < objectsAndMoveData.Length; objectsIndex++)
            {
                GameObject copyingObject = objectsAndMoveData[objectsIndex].copyObject;
                GameObject objectToCopy = objectsAndMoveData[objectsIndex].copyDemonObject;
                bool isDemonTransitionInProgress = objectsAndMoveData[objectsIndex].demonTransitioning;

                if (objectToCopy != null && !isDemonTransitionInProgress) 
                {
                    objectsAndMoveData[objectsIndex].demonTransitioning = true;
                    TweenDemonTransition(copyingObject, objectToCopy);
                }
            }
        }
    }
    public void CharacterBecomeDemon() 
    {
        raveDemonReveal = true;
    }

    private void TweenDemonTransition(GameObject copyObject, GameObject objectToCopy) 
    {
        float tweenDuration = clockCounter.Get_Q_BeatInterval() * 4;
        copyObject.transform.DOMove(objectToCopy.transform.position, tweenDuration).SetEase(Ease.InOutElastic);
    }
}
