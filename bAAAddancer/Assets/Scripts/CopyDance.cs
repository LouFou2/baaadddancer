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
        public Vector3 initialRootPosition;
        public Vector3[] targetRecordedPositions;
        public Quaternion[] targetRecordedRotations;
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

    //For the Count in...
    private bool isCountingIn = true;
    private int countInIndex = -1;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }
    private void OnEnable()
    {
        playerControls.Enable();
        ClockCounter.On_Q_Beat_Trigger += On_Q_BeatHandler; // Subscribe to the beat trigger event
    }
    private void OnDisable()
    {
        playerControls.Disable();
        ClockCounter.On_Q_Beat_Trigger -= On_Q_BeatHandler; // Subscribe to the beat trigger event
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

        //grab the initial root position (before any position changes)
        objectsAndMoveData[0].initialRootPosition = objectsAndMoveData[0].copyObject.transform.position; //*** the script that re-positions characters at the start of the scene might mess this up

        objectsAndMoveData[0].targetRecordedRotations = new Quaternion[objectsAndMoveData[0].recordingDataSO.recordedRotations.Length]; //only root rotates, so we can only initialise for the first index object (the root)

        // Adjust Vector3 positions according to offsets
        for (int i = objectsAndMoveData.Length - 1; i > -1; i--) // for each object referenced in the script // we count down so we can do 0 last*
        {
            objectsAndMoveData[i].recordingDataSO = objectsAndMoveData[i].recDataObjSequencer.currentRoundRecData;
            
            //initialising the positions arrays
            objectsAndMoveData[i].targetRecordedPositions = new Vector3[objectsAndMoveData[i].recordingDataSO.recordedPositions.Length];
            objectsAndMoveData[i].offset = new Vector3[objectsAndMoveData[i].recordingDataSO.recordedPositions.Length];

            objectsAndMoveData[i].demonTransitioning = false;

            // for each recorded Vector3 position
            for (int j = 0; j < objectsAndMoveData[i].targetRecordedPositions.Length; j++)
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
                    objectsAndMoveData[i].targetRecordedPositions[j] = new Vector3(adjustedX, adjustedY, adjustedZ);// + objectsAndMoveData[0].recordingDataSO.recordedPositions[j]; //*0 is root
                }
                //we add offset to each root object's positions array AFTER adding the recorded root positions to all the other objects' arrays
                //index 0 is the root control: the root object uses world space (transform.position)
                if (i == 0)
                {
                    objectsAndMoveData[0].targetRecordedPositions[j] // each recorded position of the root object 
                    = objectsAndMoveData[0].recordingDataSO.recordedPositions[j]
                    + objectsAndMoveData[0].initialRootPosition; // add the position of this characters root object as offset

                    objectsAndMoveData[0].targetRecordedRotations[j] = objectsAndMoveData[0].recordingDataSO.recordedRotations[j];
                }
            }
        }
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

        objectsAndMoveData[0].targetRecordedRotations = new Quaternion[objectsAndMoveData[0].recordingDataSO.recordedRotations.Length]; //only the root rotates

        // Adjust Vector3 positions according to offsets
        for (int i = objectsAndMoveData.Length - 1; i > -1; i--) // for each object referenced in the script // we count down so we can do 0 last*
        {
            objectsAndMoveData[i].recordingDataSO = objectsAndMoveData[i].recDataObjSequencer.recordingDataOfRounds[roundSwitcherIndex];
            objectsAndMoveData[i].targetRecordedPositions = new Vector3[objectsAndMoveData[i].recordingDataSO.recordedPositions.Length];

            for (int j = 0; j < objectsAndMoveData[i].targetRecordedPositions.Length; j++) 
            {
                if (i > 0)
                {
                    // introduce "noise" to offsets?
                    float adjustedX = objectsAndMoveData[i].recordingDataSO.recordedPositions[j].x + objectsAndMoveData[i].offset[j].x; //using offsets stored in Start
                    float adjustedY = objectsAndMoveData[i].recordingDataSO.recordedPositions[j].y + objectsAndMoveData[i].offset[j].y;
                    float adjustedZ = objectsAndMoveData[i].recordingDataSO.recordedPositions[j].z + objectsAndMoveData[i].offset[j].z;
                    objectsAndMoveData[i].targetRecordedPositions[j] = new Vector3(adjustedX, adjustedY, adjustedZ);// + objectsAndMoveData[0].recordingDataSO.recordedPositions[j]; //*0 is root
                }
                if (i == 0)
                {
                    objectsAndMoveData[0].targetRecordedPositions[j] // each recorded position of the root object 
                        = objectsAndMoveData[0].recordingDataSO.recordedPositions[j]
                        + objectsAndMoveData[0].initialRootPosition; // add the position of this characters root object as offset

                    objectsAndMoveData[0].targetRecordedRotations[j] = objectsAndMoveData[0].recordingDataSO.recordedRotations[j];
                }
            }
        }

        updatingRoundSequence = false;
    }

    private void On_Q_BeatHandler()
    {
        if (isCountingIn)
        {
            countInIndex++;
            if (countInIndex == 20) // 4* 4 qbeats = 16 ..+4 -> to start on the next beat ...=20
            {
                isCountingIn = false;
            }
        }
        
        if (!isCountingIn)
        {
            if (!raveDemonReveal)
            {
                Quaternion[] targetRecordedRotations = objectsAndMoveData[0].targetRecordedRotations;

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

                    Vector3 rootBeatRotation = targetRecordedRotations[targetBeatIndex].eulerAngles;

                    if (objectsIndex == 0) // if its the Root Object, we use world space
                    {
                        copyingObject.transform.DOMove(targetRecordedPositions[targetBeatIndex], tweenDuration).SetEase(Ease.Linear);
                        copyingObject.transform.DORotate(rootBeatRotation, tweenDuration).SetEase(Ease.Linear);
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
