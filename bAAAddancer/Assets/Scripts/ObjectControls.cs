using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.VFX;
using System;

public class ObjectControls : MonoBehaviour
{
    private PlayerControls playerControls;
    private ClockCounter clockCounter;
    
    private GameObject controlObject; // the actual control object
    [SerializeField] GameObject controlGizmoObject; // the gizmo of the control object (to visualise if its active)
    [SerializeField] private RecordingData recordingDataSO;
    [SerializeField] private bool leftObject = true;
    [SerializeField] private bool rightObject = false;
    private ViewSwitcher viewSwitcher;
    [SerializeField] private float x_Range = 0.5f;
    [SerializeField] private float y_Range = 0.5f;
    [SerializeField] private float z_Range = 0.5f;
    private Vector2 moveInput;

    public bool isActive = false;
    public bool useRecordedPositions = false;

    private Vector3 initialPosition;
    private Vector3 currentRecordedPosition;
    private Vector3 finalUpdatePosition;

    private void Awake()
    {
        playerControls = new PlayerControls();
        controlObject = gameObject;
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

    private void Start()
    {
        clockCounter = FindObjectOfType<ClockCounter>(); // Find the ClockCounter script in the scene
        viewSwitcher = FindObjectOfType<ViewSwitcher>();

        initialPosition = controlObject.transform.position;

        controlGizmoObject.SetActive(false);
    }
    void Update()
    {
        Vector3 currentPosition = controlObject.transform.position;
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

            // map input to limit ranges
            float rangedX = 0f;
            float rangedY = 0f;
            float rangedZ = 0f;

            Vector3 rangedPosition = Vector3.zero;
            float clampedX = 0f;
            float clampedY = 0f;
            float clampedZ = 0f;

            switch (viewSwitcher.CurrentView)  // Access the current view from ViewSwitcher
            {
                case ViewSwitcher.ViewSwitch.front:
                    rangedX = Mathf.Lerp(-x_Range, x_Range, (moveInput.x + 1f) / 2f);
                    rangedY = Mathf.Lerp(-y_Range, y_Range, (moveInput.y + 1f) / 2f); //The move input axis used changes with Views
                    rangedPosition = new Vector3(-rangedX + initialPosition.x, rangedY + initialPosition.y, initialPosition.z);
                    clampedX = Mathf.Clamp(rangedPosition.x, initialPosition.x - x_Range, initialPosition.x + x_Range);
                    clampedY = Mathf.Clamp(rangedPosition.y, initialPosition.y - y_Range, initialPosition.y + y_Range);
                    finalUpdatePosition = new Vector3(clampedX, clampedY, rangedPosition.z);
                    break;
                case ViewSwitcher.ViewSwitch.top:
                    // Handle top view controls
                    rangedX = Mathf.Lerp(-x_Range, x_Range, (moveInput.x + 1f) / 2f);
                    rangedZ = Mathf.Lerp(-z_Range, z_Range, (moveInput.y + 1f) / 2f);
                    rangedZ = -rangedZ; // top view "mirrors" the z axis (+y input = -z movement)
                    rangedPosition = new Vector3(-rangedX + initialPosition.x, initialPosition.y, rangedZ + initialPosition.z);
                    clampedX = Mathf.Clamp(rangedPosition.x, initialPosition.x - x_Range, initialPosition.x + x_Range);
                    clampedZ = Mathf.Clamp(rangedPosition.z, initialPosition.z - z_Range, initialPosition.z + z_Range);
                    finalUpdatePosition = new Vector3(clampedX, rangedPosition.y, clampedZ);
                    break;
                case ViewSwitcher.ViewSwitch.left:
                    // Handle left view controls
                    rangedY = Mathf.Lerp(-y_Range, y_Range, (moveInput.y + 1f) / 2f);
                    rangedZ = Mathf.Lerp(-z_Range, z_Range, (moveInput.x + 1f) / 2f);
                    rangedPosition = new Vector3(initialPosition.x, rangedY + initialPosition.y, rangedZ + initialPosition.z);
                    clampedY = Mathf.Clamp(rangedPosition.y, initialPosition.y - y_Range, initialPosition.y + y_Range);
                    clampedZ = Mathf.Clamp(rangedPosition.z, initialPosition.z - z_Range, initialPosition.z + z_Range);
                    finalUpdatePosition = new Vector3(rangedPosition.x, clampedY, clampedZ);
                    break;
                case ViewSwitcher.ViewSwitch.right:
                    // Handle right view controls
                    rangedY = Mathf.Lerp(-y_Range, y_Range, (moveInput.y + 1f) / 2f);
                    rangedZ = Mathf.Lerp(-z_Range, z_Range, (moveInput.x + 1f) / 2f);
                    rangedZ = -rangedZ; // right view also flips the z axis (+x input = -z movement)
                    rangedPosition = new Vector3(initialPosition.x, rangedY + initialPosition.y, rangedZ + initialPosition.z);
                    clampedY = Mathf.Clamp(rangedPosition.y, initialPosition.y - y_Range, initialPosition.y + y_Range);
                    clampedZ = Mathf.Clamp(rangedPosition.z, initialPosition.z - z_Range, initialPosition.z + z_Range);
                    finalUpdatePosition = new Vector3(rangedPosition.x, clampedY, clampedZ);
                    break;
                default:
                    viewSwitcher.CurrentView = ViewSwitcher.ViewSwitch.front; // Set default view to front if the current view is not recognized
                    break;
            }
            controlObject.transform.position = finalUpdatePosition;
        }
    }
    public Vector3 GetPositionToRecord()
    {
        return finalUpdatePosition;
    }
    void On_Q_BeatHandler()
    {
        if (useRecordedPositions)
        {
            int currentPositionIndex = clockCounter.GetCurrent_Q_Beat(); //the indexes of the positions corresponds to the beats (the beats are used to record them)
            int targetPositionIndex = currentPositionIndex + 1;
            if (targetPositionIndex > recordingDataSO.recordedPositions.Length - 1) targetPositionIndex = 0;
            if (targetPositionIndex < 0) targetPositionIndex = 0;
            float tweenDuration = clockCounter.Get_Q_BeatInterval();

            // Tween the object's position to the next recorded position
            controlObject.transform.DOMove(recordingDataSO.recordedPositions[targetPositionIndex], tweenDuration).SetEase(Ease.Linear);
        }
    }


}
