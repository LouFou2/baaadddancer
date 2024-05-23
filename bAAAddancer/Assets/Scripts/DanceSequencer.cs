using UnityEngine;

public class DanceSequencer : MonoBehaviour
{
    private ClockCounter clockCounter;
    private int beatCount = 0;
    [SerializeField] private RoundsRecData[] recDataObjSequencers; // Arrays (representing the game rounds) that hold Rec Data SO's
    [SerializeField] private RecordingData[] recordingDataObjects; // Array to store recording data for each GameObject
    [SerializeField] private ObjectControls[] objControlScripts;
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

        int currentRound = GameManager.Instance.GetCurrentRound();

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
        // === CHECKING INPUT === //
        //bool record = playerControls.DanceControls.Record.IsPressed(); //***currently removed the record flag
        //bool record = false;

        //Vector2 inputVector2 = playerControls.DanceControls.MoveL.ReadValue<Vector2>();

        //if(inputVector2.magnitude > 0.01f) record = true;

        for (int i = 0; i < objControlScripts.Length; i++)
        {
            if (objControlScripts[i].isActive)// && record)
            {
                objControlScripts[i].useRecordedPositions = false;
                // Record the position + rotation for the current GameObject for the current beat
                recordingDataObjects[i].recordedPositions[beatCount] = objControlScripts[i].GetPositionToRecord();
                //recordingDataObjects[i].recordedRotations[beatCount] = objControlScripts[i].GetRotationToRecord();
            }
        }
    }
}
