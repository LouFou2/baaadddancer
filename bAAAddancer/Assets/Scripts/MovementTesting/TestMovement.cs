using UnityEngine;
using DG.Tweening;

public class TestMovement : MonoBehaviour
{
    private PlayerControls playerControls;
    private ClockCounter clockCounter;

    [System.Serializable]
    public class MoveObject 
    {
        public GameObject leftControlObject;
        public GameObject rightControlObject;
        public bool isActive = false;

        public bool useMovementLimit = false;
        public float x_RangeMin = -0.5f;
        public float x_RangeMax = 0.5f;
        public float y_RangeMin = -0.5f;
        public float y_RangeMax = 0.5f;
        public float z_RangeMin = -0.5f;
        public float z_RangeMax = 0.5f;
        public float moveSpeed = 2f;

        public Vector3 initialPosition_L;
        public Vector3 previousPosition2_L;
        public Vector3 previousPosition1_L;
        public Vector3 currentPosition_L;

        public Vector3 initialPosition_R;
        public Vector3 previousPosition2_R;
        public Vector3 previousPosition1_R;
        public Vector3 currentPosition_R;
    }
    [SerializeField] private MoveObject[] moveObjects;

    private Vector2 moveInputL;
    private Vector2 moveInputR;

    private int controlObjectIndex = 0;
    private bool switchControlObjectPrevious;
    private bool switchControlObjectNext;
    
    private void Awake()
    {
        playerControls = new PlayerControls();
    }
    private void OnEnable()
    {
        playerControls.Enable();
        //ClockCounter.On_Q_BeatTrigger += On_Q_BeatHandler; // Subscribe to the beat trigger event
    }
    private void OnDisable()
    {
        playerControls.Disable();
        //ClockCounter.On_Q_BeatTrigger -= On_Q_BeatHandler; // Subscribe to the beat trigger event
    }
    private void Start()
    {
        clockCounter = FindObjectOfType<ClockCounter>();

        controlObjectIndex = 0;

        foreach (MoveObject moveObject in moveObjects)
        {
            moveObject.initialPosition_L = moveObject.leftControlObject.transform.position;
            moveObject.previousPosition2_L = moveObject.initialPosition_L;
            moveObject.previousPosition1_L = moveObject.initialPosition_L;
            moveObject.currentPosition_L = moveObject.initialPosition_L;

            moveObject.initialPosition_R = moveObject.rightControlObject.transform.position;
            moveObject.previousPosition2_R = moveObject.initialPosition_L;
            moveObject.previousPosition1_R = moveObject.initialPosition_L;
            moveObject.currentPosition_R = moveObject.initialPosition_L;
        }
    }
    private void Update()
    {
        // first we determine the current active control objects
        int nextIndex = ((controlObjectIndex + 1) > 5) ? 0 : controlObjectIndex + 1 ;       // there are 5 pairs of control objects
        int previousIndex = ((controlObjectIndex - 1) < 0) ? 5 : controlObjectIndex - 1;    // also we make sure it loops 0-5                                               
        // the controlObjects gets switched by the bumpers
        for(int i = 0; i < 5; i++) 
        {
            MoveObject moveObject = moveObjects[i];
            moveObject.isActive = (i == controlObjectIndex) ? true : false;
        }
        switchControlObjectPrevious = playerControls.GenericInput.LBumper.triggered;
        switchControlObjectNext = playerControls.GenericInput.RBumper.triggered;
        if (switchControlObjectPrevious) controlObjectIndex = previousIndex;
        if (switchControlObjectNext) controlObjectIndex = nextIndex;

        Debug.Log(controlObjectIndex);
        /*
        // activating the gizmo/control/visualiser object
        if (!isActive)
        {
            useRecordedPositions = true;  // *** MIGHT HAVE TO SET THIS FALSE IN OTHER CONDITION>> CHECK LATER
            if (controlGizmoObject != null)
                controlGizmoObject.SetActive(false);
        }
        else
        {
            useRecordedPositions = false; //*** TRYING IT HERE
            if (controlGizmoObject != null)
                controlGizmoObject.SetActive(true);
        }

        isRecording = false;

        // checking controller input = isRecording
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

        // get moveInput value from controller
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

        controlObject.transform.position = currentRecordedPosition;

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
                case ViewSwitcher.ViewSwitch.front:
                    // NB: audience view (controller input) is mirror of character orientation (so:(+x input = -x position movement)
                    // swap the range max and min values so object movement in world space corresponds with controller input:
                    // also, if input is negative, we lerp from object default to object min position using negated input value
                    // (lerp uses 0-1 range) so if input value is nagative, we make it positive (-input)

                    
                        //rangedX = (moveInput.x <= 0) ? Mathf.Lerp(0, x_RangeMax, -moveInput.x) : Mathf.Lerp(0, x_RangeMin, moveInput.x);
                       //rangedY = (moveInput.y <= 0) ? Mathf.Lerp(0, y_RangeMin, -moveInput.y) : Mathf.Lerp(0, y_RangeMax, moveInput.y);

                        rangedX = Mathf.Clamp(-moveInput.x, x_RangeMin, x_RangeMax); // Assuming x_RangeMax is positive
                        rangedY = Mathf.Clamp(moveInput.y, y_RangeMin, y_RangeMax); // Assuming y_RangeMax is positive

                        rangedPosition = new Vector3(rangedX + initialPosition.x, rangedY + initialPosition.y, currentRecordedPosition.z); // note: we use the current position for the uncontrolled axis

                        clampedX = Mathf.Clamp(rangedPosition.x, initialPosition.x + x_RangeMin, initialPosition.x + x_RangeMax); // but: initial position to set the clamp range
                        clampedY = Mathf.Clamp(rangedPosition.y, initialPosition.y + y_RangeMin, initialPosition.y + y_RangeMax);

                        finalUpdatePosition = new Vector3(clampedX, clampedY, currentRecordedPosition.z); // current position again for the uncontrolled axis
                    
                    

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
            }
            else
            {
                if (leftObject && opposingObject != null)
                {
                    float limitedX = Mathf.Clamp(finalUpdatePosition.x, x_RangeMin, opposingObject.transform.position.x);

                    finalUpdatePosition = new Vector3(limitedX, finalUpdatePosition.y, finalUpdatePosition.z);
                    controlObject.transform.position = finalUpdatePosition;
                }
                else if (rightObject && opposingObject != null)
                {
                    float limitedX = Mathf.Clamp(finalUpdatePosition.x, opposingObject.transform.position.x, x_RangeMax);
                    finalUpdatePosition = new Vector3(limitedX, finalUpdatePosition.y, finalUpdatePosition.z);
                    controlObject.transform.position = finalUpdatePosition;
                }
                else
                {
                    controlObject.transform.position = finalUpdatePosition;
                }
            }

        }*/
    }
    /*
    public Vector3 GetPositionToRecord()
    {
        return finalUpdatePosition;
    }
    void On_Q_BeatHandler()
    {
        int currentPositionIndex = clockCounter.GetCurrent_Q_Beat(); //the indexes of the positions corresponds to the beats (the beats are used to record them)
        int targetPositionIndex = currentPositionIndex + 1;

        // index can loop/wrap around:
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
        // Note: it is not actually moving the object, only the Vector3 (currentRecordedPosition) that can be referred to in the Update method
        float xTarget = tweenTargetPosition.x;
        float yTarget = tweenTargetPosition.y;
        float zTarget = tweenTargetPosition.z;
        DOTween.To(() => currentRecordedPosition, x => currentRecordedPosition = x, new Vector3(xTarget, yTarget, zTarget), tweenDuration).SetEase(Ease.Linear);

    }*/
}
