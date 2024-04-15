using UnityEngine;
using DG.Tweening;

public class ObjectControls : MonoBehaviour
{
    private PlayerControls playerControls;
    private ClockCounter clockCounter;
    private ViewSwitcher viewSwitcher;

    private GameObject controlObject; // the actual control object
    [SerializeField] GameObject controlGizmoObject; // the gizmo of the control object (to visualise if its active)
    [SerializeField] private RecordingData recordingDataSO;

    [SerializeField] private bool leftObject = true;
    [SerializeField] private bool rightObject = false;
    [SerializeField] private GameObject opposingObject; // assign other left/right object
    [SerializeField] private bool useMovementLimit = false;

    [SerializeField] private float x_RangeMin = -0.5f;
    [SerializeField] private float x_RangeMax = 0.5f;
    [SerializeField] private float y_RangeMin = -0.5f;
    [SerializeField] private float y_RangeMax = 0.5f;
    [SerializeField] private float z_RangeMin = -0.5f;
    [SerializeField] private float z_RangeMax = 0.5f;

    private Vector2 moveInput;

    public bool isActive = false;
    public bool isRecording = false;
    public bool useRecordedPositions = false;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 currentRecordedPosition;
    private Quaternion currentRecordedRotation;
    private Vector3 finalUpdatePosition;
    private Quaternion finalUpdateRotation;

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
        clockCounter = FindObjectOfType<ClockCounter>();
        viewSwitcher = FindObjectOfType<ViewSwitcher>();

        initialPosition = controlObject.transform.position;
        initialRotation = controlObject.transform.rotation;
        currentRecordedPosition = initialPosition;
        currentRecordedRotation = initialRotation;

        controlGizmoObject.SetActive(false);
    }
    private void Update()
    {
        if (!isActive)
        {
            useRecordedPositions = true;
            controlGizmoObject.SetActive(false);
        }
        else 
        {
            controlGizmoObject.SetActive(true);
        }

        isRecording = false;

        if (leftObject) 
        {
            Vector2 inputVector2 = playerControls.DanceControls.MoveL.ReadValue<Vector2>();
            if (inputVector2.magnitude > 0.001f) isRecording = true; // turn on recording if input is above a threshold
        }
        if (rightObject)
        {
            Vector2 inputVector2 = playerControls.DanceControls.MoveR.ReadValue<Vector2>();
            if (inputVector2.magnitude > 0.001f) isRecording = true; // turn on recording if input is above a threshold
        }

        if (isActive && isRecording)
        {
            
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
        controlObject.transform.position = currentRecordedPosition;
        controlObject.transform.rotation = currentRecordedRotation;

        finalUpdateRotation = currentRecordedRotation; // *** MIGHT USE ROTATION, IN WHICH CASE ADD INTO LOGIC BELOW (instead of this line)

        if (isActive && isRecording)
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
                /*case ViewSwitcher.ViewSwitch.front:
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
                    clampedX = Mathf.Clamp(rangedPosition.x, initialPosition.x + x_RangeMin, initialPosition.x + x_RangeMax);
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
                    break;*/

                case ViewSwitcher.ViewSwitch.front:
                    // NB: audience view (controller input) is mirror of character orientation (so:(+x input = -x position movement)
                    // swap the range max and min values so object movement in world space corresponds with controller input:
                    // also, if input is negative, we lerp from object default to object min position using negated input value
                    // (lerp uses 0-1 range) so if input value is nagative, we make it positive (-input)

                    rangedX = (moveInput.x <= 0) ? Mathf.Lerp(0, x_RangeMax, -moveInput.x) : Mathf.Lerp(0, x_RangeMin, moveInput.x);
                    rangedY = (moveInput.y <= 0) ? Mathf.Lerp(0, y_RangeMin, -moveInput.y) : Mathf.Lerp(0, y_RangeMax, moveInput.y);

                    rangedPosition = new Vector3(rangedX + initialPosition.x, rangedY + initialPosition.y, currentRecordedPosition.z); // note: we use the current position for the uncontrolled axis

                    clampedX = Mathf.Clamp(rangedPosition.x, initialPosition.x + x_RangeMin, initialPosition.x + x_RangeMax); // but: initial position to set the clamp range
                    clampedY = Mathf.Clamp(rangedPosition.y, initialPosition.y + y_RangeMin, initialPosition.y + y_RangeMax);

                    finalUpdatePosition = new Vector3(clampedX, clampedY, currentRecordedPosition.z); // current position again for the uncontrolled axis 
                    break;

                case ViewSwitcher.ViewSwitch.top:
                    rangedX = (moveInput.x <= 0) ? Mathf.Lerp(0, x_RangeMax, -moveInput.x) : Mathf.Lerp(0, x_RangeMin, moveInput.x);
                    rangedZ = (moveInput.y <= 0) ? Mathf.Lerp(0, z_RangeMax, -moveInput.y) : Mathf.Lerp(0, z_RangeMin, moveInput.y);

                    rangedPosition = new Vector3(rangedX + initialPosition.x, currentRecordedPosition.y, rangedZ + initialPosition.z);
                    clampedX = Mathf.Clamp(rangedPosition.x, initialPosition.x + x_RangeMin, initialPosition.x + x_RangeMax);
                    clampedZ = Mathf.Clamp(rangedPosition.z, initialPosition.z + z_RangeMin, initialPosition.z + z_RangeMax);

                    finalUpdatePosition = new Vector3(clampedX, currentRecordedPosition.y, clampedZ);
                    break;

                case ViewSwitcher.ViewSwitch.left:
                    // Handle left view controls
                    rangedY = (moveInput.y <= 0) ? Mathf.Lerp(0, y_RangeMin, -moveInput.y) : Mathf.Lerp(0, y_RangeMax, moveInput.y);
                    rangedZ = (moveInput.x <= 0) ? Mathf.Lerp(0, z_RangeMin, -moveInput.x) : Mathf.Lerp(0, z_RangeMax, moveInput.x);

                    rangedPosition = new Vector3(currentRecordedPosition.x, rangedY + initialPosition.y, rangedZ + initialPosition.z);
                    clampedY = Mathf.Clamp(rangedPosition.y, initialPosition.y + y_RangeMin, initialPosition.y + y_RangeMax);
                    clampedZ = Mathf.Clamp(rangedPosition.z, initialPosition.z + z_RangeMin, initialPosition.z + z_RangeMax);

                    finalUpdatePosition = new Vector3(currentRecordedPosition.x, clampedY, clampedZ);
                    break;

                case ViewSwitcher.ViewSwitch.right:
                    // Handle right view controls
                    rangedY = (moveInput.y <= 0) ? Mathf.Lerp(0, y_RangeMin, -moveInput.y) : Mathf.Lerp(0, y_RangeMax, moveInput.y);
                    rangedZ = (moveInput.x <= 0) ? Mathf.Lerp(0, z_RangeMax, -moveInput.x) : Mathf.Lerp(0, z_RangeMin, moveInput.x);

                    rangedPosition = new Vector3(currentRecordedPosition.x, rangedY + initialPosition.y, rangedZ + initialPosition.z);
                    clampedY = Mathf.Clamp(rangedPosition.y, initialPosition.y + y_RangeMin, initialPosition.y + y_RangeMax);
                    clampedZ = Mathf.Clamp(rangedPosition.z, initialPosition.z + z_RangeMin, initialPosition.z + z_RangeMax);

                    finalUpdatePosition = new Vector3(currentRecordedPosition.x, clampedY, clampedZ);
                    break;

                default:
                    viewSwitcher.CurrentView = ViewSwitcher.ViewSwitch.front; // Set default view to front if the current view is not recognized
                    break;
            }

            if (!useMovementLimit) // logic below: stop objects from crossing over each other
            {
                controlObject.transform.position = finalUpdatePosition;
                controlObject.transform.rotation = finalUpdateRotation;
            }
            else 
            {
                if (leftObject && opposingObject != null) 
                {
                    float limitedX = Mathf.Clamp(finalUpdatePosition.x, x_RangeMin, opposingObject.transform.position.x);

                    finalUpdatePosition = new Vector3(limitedX, finalUpdatePosition.y, finalUpdatePosition.z);
                    controlObject.transform.position = finalUpdatePosition;
                    controlObject.transform.rotation = finalUpdateRotation;
                }
                else if (rightObject && opposingObject != null) 
                { 
                    float limitedX = Mathf.Clamp(finalUpdatePosition.x, opposingObject.transform.position.x, x_RangeMax);
                    finalUpdatePosition = new Vector3(limitedX, finalUpdatePosition.y, finalUpdatePosition.z);
                    controlObject.transform.position = finalUpdatePosition;
                    controlObject.transform.rotation = finalUpdateRotation;
                }
                else 
                {
                    controlObject.transform.position = finalUpdatePosition;
                    controlObject.transform.rotation = finalUpdateRotation;
                }
            }
            
        }
    }
    public Vector3 GetPositionToRecord()
    {
        return finalUpdatePosition;
    }
    public Quaternion GetRotationToRecord()
    {
        return finalUpdateRotation;
    }
    void On_Q_BeatHandler()
    {
        int currentPositionIndex = clockCounter.GetCurrent_Q_Beat(); //the indexes of the positions corresponds to the beats (the beats are used to record them)
        int targetPositionIndex = currentPositionIndex + 1;
        if (targetPositionIndex > recordingDataSO.recordedPositions.Length - 1) targetPositionIndex = 0;
        if (targetPositionIndex < 0) targetPositionIndex = 0;
        float tweenDuration = clockCounter.Get_Q_BeatInterval();
        Vector3 tweenTargetPosition = recordingDataSO.recordedPositions[targetPositionIndex];

        if (useRecordedPositions)
        {
            // Tween the object's position to the next recorded position
            controlObject.transform.DOMove(tweenTargetPosition, tweenDuration).SetEase(Ease.Linear);
        }
        // This tween is moving a Vector3 that can be used for axis not controlled by player input (depending on view switching)
        // Note: it is not actually moving the object, only the Vector3 (currentRecordedPosition) that can be referred to in the Fixed Update method
        float xTarget = tweenTargetPosition.x;
        float yTarget = tweenTargetPosition.y;
        float zTarget = tweenTargetPosition.z;
        DOTween.To(() => currentRecordedPosition, x => currentRecordedPosition = x, new Vector3(xTarget, yTarget, zTarget), tweenDuration).SetEase(Ease.Linear);
    }


}
