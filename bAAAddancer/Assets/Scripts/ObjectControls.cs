using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.VFX;

public class ObjectControls : MonoBehaviour
{
    private PlayerControls playerControls;
    private GameObject controlObject;
    private ClockCounter clockCounter;
    [SerializeField] GameObject controlGizmoObject;
    [SerializeField] private RecordingData recordingDataSO;
    [SerializeField] private bool leftObject = true;
    [SerializeField] private bool rightObject = false;
    [SerializeField] private float x_Range;
    [SerializeField] private float y_Range;
    private Vector2 moveInput;

    public bool isActive = false;
    public bool useRecordedPositions = false;

    private Vector3 initialPosition;

    private void Awake()
    {
        playerControls = new PlayerControls();
        controlObject = gameObject;
    }
    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Start()
    {
        clockCounter = FindObjectOfType<ClockCounter>(); // Find the ClockCounter script in the scene

        initialPosition = controlObject.transform.position;

        controlGizmoObject.SetActive(false);
    }
    void Update()
    {
        // Check if the playback trigger is pressed
        if (playerControls.DanceControls.Play.triggered && isActive)
        {
            // Toggle the useRecordedPositions boolean
            useRecordedPositions = !useRecordedPositions;
        }

        controlGizmoObject.SetActive(false);

        if (isActive && !useRecordedPositions)
        {
            controlGizmoObject.SetActive(true);
            if (leftObject) 
            {
                rightObject = false;
                moveInput = playerControls.DanceControls.MoveL.ReadValue<Vector2>();
            }

            if (rightObject) 
            {
                leftObject = false;
                moveInput = playerControls.DanceControls.MoveR.ReadValue<Vector2>();
            }

            float rangedX = Mathf.Lerp(-x_Range, x_Range, (moveInput.x + 1f) / 2f);
            float rangedY = Mathf.Lerp(-y_Range, y_Range, (moveInput.y + 1f) / 2f);

            controlObject.transform.position = new Vector3(-rangedX + initialPosition.x, rangedY + initialPosition.y, 0f);

            Vector3 currentPosition = controlObject.transform.position;
            float newX = Mathf.Clamp(currentPosition.x, initialPosition.x - x_Range, initialPosition.x + x_Range);
            float newY = Mathf.Clamp(currentPosition.y, initialPosition.y - y_Range, initialPosition.y + y_Range);
            controlObject.transform.position = new Vector3(newX, newY, currentPosition.z);
        }

        if (useRecordedPositions) 
        {
            int currentPositionIndex = clockCounter.GetCurrent_Q_Beat(); //the indexes of the positions corresponds to the beats (the beats are used to record them)
            int targetPositionIndex = currentPositionIndex + 1;
            if (targetPositionIndex > recordingDataSO.recordedPositions.Length -1) targetPositionIndex = 0;
            if (targetPositionIndex < 0) targetPositionIndex = 0;
            float tweenDuration = clockCounter.GetBeatInterval();

            // Tween the object's position to the next recorded position
            controlObject.transform.DOMove(recordingDataSO.recordedPositions[targetPositionIndex], tweenDuration);
        }
    }

    
}
