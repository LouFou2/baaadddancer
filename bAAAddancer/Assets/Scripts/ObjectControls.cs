using UnityEngine;
using DG.Tweening;

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
    [SerializeField] private float x_RangeMin = -0.5f;
    [SerializeField] private float x_RangeMax = 0.5f;
    [SerializeField] private float y_RangeMin = -0.5f;
    [SerializeField] private float y_RangeMax = 0.5f;
    [SerializeField] private float z_RangeMin = -0.5f;
    [SerializeField] private float z_RangeMax = 0.5f;


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
    private void Update()
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

        }
    }
    void FixedUpdate()
    {
        Vector3 currentPosition = controlObject.transform.position;

        
        if (isActive && !useRecordedPositions)
        {

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
                    // NB: audience view (controller input) is mirror of character orientation (so:(+x input = -x position movement)
                    // swap the range max and min values so object movement in world space corresponds with controller input:
                    // also, if input is negative, we lerp from object default to object min position using negated input value
                    // (lerp uses 0-1 range) so if input value is nagative, we make it positive (-input)
                    rangedX = (moveInput.x <= 0)? Mathf.Lerp(0, x_RangeMax, -moveInput.x) : Mathf.Lerp(0, x_RangeMin, moveInput.x);
                    rangedY = (moveInput.y <= 0) ? Mathf.Lerp(0, y_RangeMin, -moveInput.y) : Mathf.Lerp(0, y_RangeMax, moveInput.y);

                    rangedPosition = new Vector3(rangedX + initialPosition.x, rangedY + initialPosition.y, initialPosition.z); 
                    clampedX = Mathf.Clamp(rangedPosition.x, initialPosition.x + x_RangeMin, initialPosition.x + x_RangeMax);
                    clampedY = Mathf.Clamp(rangedPosition.y, initialPosition.y + y_RangeMin, initialPosition.y + y_RangeMax);
                    finalUpdatePosition = new Vector3(clampedX, clampedY, initialPosition.z);
                    break;

                case ViewSwitcher.ViewSwitch.top:
                    rangedX = (moveInput.x <= 0) ? Mathf.Lerp(0, x_RangeMax, -moveInput.x) : Mathf.Lerp(0, x_RangeMin, moveInput.x);
                    rangedZ = (moveInput.y <= 0) ? Mathf.Lerp(0, z_RangeMax, -moveInput.y) : Mathf.Lerp(0, z_RangeMin, moveInput.y);

                    rangedPosition = new Vector3(rangedX + initialPosition.x, initialPosition.y, rangedZ + initialPosition.z);
                    clampedX = Mathf.Clamp(rangedPosition.x, initialPosition.x + x_RangeMin, initialPosition.x + x_RangeMax);// -rangedPosition.x
                    clampedZ = Mathf.Clamp(rangedPosition.z, initialPosition.z + z_RangeMin, initialPosition.z + z_RangeMax);
                    finalUpdatePosition = new Vector3(clampedX, initialPosition.y, clampedZ);
                    break;

                case ViewSwitcher.ViewSwitch.left:
                    // Handle left view controls
                    rangedY = (moveInput.y <= 0) ? Mathf.Lerp(0, y_RangeMin, -moveInput.y) : Mathf.Lerp(0, y_RangeMax, moveInput.y);
                    rangedZ = (moveInput.x <= 0) ? Mathf.Lerp(0, z_RangeMin, -moveInput.x) : Mathf.Lerp(0, z_RangeMax, moveInput.x);

                    rangedPosition = new Vector3(initialPosition.x, rangedY + initialPosition.y, rangedZ + initialPosition.z);
                    clampedY = Mathf.Clamp(rangedPosition.y, initialPosition.y + y_RangeMin, initialPosition.y + y_RangeMax);
                    clampedZ = Mathf.Clamp(rangedPosition.z, initialPosition.z + z_RangeMin, initialPosition.z + z_RangeMax);
                    finalUpdatePosition = new Vector3(initialPosition.x, clampedY, clampedZ);
                    break;

                case ViewSwitcher.ViewSwitch.right:
                    // Handle right view controls
                    rangedY = (moveInput.y <= 0) ? Mathf.Lerp(0, y_RangeMin, -moveInput.y) : Mathf.Lerp(0, y_RangeMax, moveInput.y);
                    rangedZ = (moveInput.x <= 0) ? Mathf.Lerp(0, z_RangeMax, -moveInput.x) : Mathf.Lerp(0, z_RangeMin, moveInput.x);

                    rangedPosition = new Vector3(initialPosition.x, rangedY + initialPosition.y, rangedZ + initialPosition.z);
                    clampedY = Mathf.Clamp(rangedPosition.y, initialPosition.y + y_RangeMin, initialPosition.y + y_RangeMax);
                    clampedZ = Mathf.Clamp(rangedPosition.z, initialPosition.z + z_RangeMin, initialPosition.z + z_RangeMax);
                    finalUpdatePosition = new Vector3(initialPosition.x, clampedY, clampedZ);
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
