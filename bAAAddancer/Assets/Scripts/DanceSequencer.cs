using UnityEngine;

public class DanceSequencer : MonoBehaviour
{
    private ClockCounter clockCounter;
    private int beatCount = 0;
    [SerializeField] private RecordingData[] recordingDataArray; // Array to store recording data for each GameObject
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

        // For each Control Object:
        // Initialize the array of Vector3 positions and fill each element with the starting position
        for (int i = 0; i < objControlScripts.Length; i++)
        {
            recordingDataArray[i].recordedPositions = new Vector3[64];
            Vector3 startingPosition = objControlScripts[i].gameObject.transform.position;
            for (int j = 0; j < recordingDataArray[i].recordedPositions.Length; j++)
            {
                recordingDataArray[i].initialPositions[j] = startingPosition;
                recordingDataArray[i].recordedPositions[j] = startingPosition;
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
        bool record = playerControls.DanceControls.Record.IsPressed();

        for (int i = 0; i < objControlScripts.Length; i++)
        {
            if (objControlScripts[i].isActive && record)
            {
                objControlScripts[i].useRecordedPositions = false;
                // Record the position for the current GameObject for the current beat
                recordingDataArray[i].recordedPositions[beatCount] = objControlScripts[i].GetPositionToRecord();
            }
        }
    }
}
