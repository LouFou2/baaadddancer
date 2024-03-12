using System.Collections;
using System.Collections.Generic;
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
        ClockCounter.OnBeatTrigger += OnBeatTriggerHandler; // Subscribe to the beat trigger event
    }
    private void OnDisable()
    {
        playerControls.Disable();
        ClockCounter.OnBeatTrigger -= OnBeatTriggerHandler; // Unsubscribe from the beat trigger event
    }

    void Start()
    {
        clockCounter = FindObjectOfType<ClockCounter>(); // Find the ClockCounter script in the scene

        recordingDataArray = new RecordingData[objControlScripts.Length]; // Initialize array size

        // Instantiate RecordingData for each GameObject
        for (int i = 0; i < objControlScripts.Length; i++)
        {
            recordingDataArray[i] = ScriptableObject.CreateInstance<RecordingData>();

            // Initialize the array of Vector3 positions and fill each element with the starting position
            recordingDataArray[i].recordedPositions = new Vector3[16];
            Vector3 startingPosition = objControlScripts[i].gameObject.transform.position;
            for (int j = 0; j < recordingDataArray[i].recordedPositions.Length; j++)
            {
                recordingDataArray[i].recordedPositions[j] = startingPosition;
            }
        }
    }

    void Update()
    {
        // === CHECKING FOR THE BEAT === //
        beatCount = clockCounter.GetCurrentBeat(); //the clock counts in quarter beats
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
                // Record the position for the current GameObject for the current beat
                recordingDataArray[i].recordedPositions[beatCount] = objControlScripts[i].gameObject.transform.position;
            }
        }
    }
}
