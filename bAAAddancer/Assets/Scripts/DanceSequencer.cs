using UnityEngine;

public class DanceSequencer : MonoBehaviour
{
    private ClockCounter clockCounter;
    private int beatCount = 0;
    [SerializeField] private RoundsRecData rootRecDataSequencer; // Arrays (representing the game rounds) that hold Rec Data SO's
    [SerializeField] private RecordingData rootRecordingData;
    [SerializeField] private RoundsRecData[] recDataObjSequencers; // Arrays (representing the game rounds) that hold Rec Data SO's
    [SerializeField] private RecordingData[] recordingDataObjects; // Array to store recording data for each GameObject
    [SerializeField] private ObjectControls[] objControlScripts;
    [SerializeField] private RootControl rootControl; // root object movement will be added, e.g. jumps and spins

    [SerializeField] private PlayerControls playerControls;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }
    private void OnEnable()
    {
        playerControls.Enable();
        ClockCounter.On_Q_BeatTrigger += OnBeatTriggerHandler; // Subscribe to the beat trigger event
    }
    private void OnDisable()
    {
        playerControls.Disable();
        ClockCounter.On_Q_BeatTrigger -= OnBeatTriggerHandler; // Unsubscribe from the beat trigger event
    }

    void Start()
    {
        clockCounter = FindObjectOfType<ClockCounter>(); // Find the ClockCounter script in the scene
        rootControl = FindObjectOfType<RootControl>();

        int currentRound = GameManager.Instance.GetCurrentRound();

        //Root Object:
        rootRecDataSequencer.currentRoundRecData = rootRecDataSequencer.recordingDataOfRounds[currentRound];
        rootRecordingData = rootRecDataSequencer.currentRoundRecData;
        for (int rootPositions = 0; rootPositions < rootRecordingData.recordedPositions.Length; rootPositions++)
        {
            rootRecordingData.initialPositions[rootPositions] = Vector3.zero;
            rootRecordingData.recordedPositions[rootPositions] = Vector3.zero;

            rootRecordingData.initialRotations[rootPositions] = Quaternion.identity;
            rootRecordingData.recordedRotations[rootPositions] = Quaternion.identity;
        }

        // For each Control Object:
        // Initialize the array of Vector3 positions and fill each element with the starting position
        for (int i = 0; i < objControlScripts.Length; i++)
        {
            //setting the currentRound RecData SO in the array that represents rounds (for the current iterated control object)
            recDataObjSequencers[i].currentRoundRecData = recDataObjSequencers[i].recordingDataOfRounds[currentRound];

            recordingDataObjects[i] = recDataObjSequencers[i].currentRoundRecData;
            recordingDataObjects[i].recordedPositions = new Vector3[64];
            Vector3 startingPosition = objControlScripts[i].gameObject.transform.position;

            for (int j = 0; j < recordingDataObjects[i].recordedPositions.Length; j++)
            {
                recordingDataObjects[i].initialPositions[j] = startingPosition;
                recordingDataObjects[i].recordedPositions[j] = startingPosition;
            }
        }
    }

    void Update()
    {
        // === CHECKING FOR THE BEAT === //
        beatCount = clockCounter.GetCurrent_Q_Beat(); //the clock counts in quarter beats
    }

    // === RECORDING === //
    void OnBeatTriggerHandler() // Method to handle beat trigger event
    {
        Vector3 rootPos = rootControl.GetRootPosition();
        Quaternion rootRot = rootControl.GetRootRotation();

        if (rootControl.isRecording)
        {
            rootRecordingData.recordedPositions[beatCount] = rootPos;
            rootRecordingData.recordedRotations[beatCount] = rootRot;
        }
        // === CHECKING INPUT === //
        for (int i = 0; i < objControlScripts.Length; i++)
        {
            if (objControlScripts[i].isActive && objControlScripts[i].isRecording)
            {
                objControlScripts[i].useRecordedPositions = false;

                recordingDataObjects[i].recordedPositions[beatCount] = objControlScripts[i].GetPositionToRecord();
            }
        }
    }
}
