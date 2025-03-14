using System.Collections;
using UnityEngine;
using DG.Tweening;

public class RootControl : MonoBehaviour
{
    private ClockCounter clockCounter;
    private PlayerControls playerControls;
    private ViewSwitcher viewSwitcher;

    private Vector3 rootPosition = Vector3.zero;
    private Quaternion rootRotation = Quaternion.identity;

    //Data SO's:
    [SerializeField] private RoundsRecData recordingDataSequencer;
    [SerializeField] private RecordingData recordingDataSO;

    [SerializeField] private bool grounded = true;
    [SerializeField] private float jumpHeight = 0.5f;
    private float jumpTime = 0f;
    [SerializeField] private float jumpDuration = 0f;

    [SerializeField] private bool isJumping = false;
    [SerializeField] private bool isMoving = false;
    [SerializeField] private bool isTurning = false;

    public bool isActive = false; // this gets set in the ControlObjectSwitcher
    public bool isRecording = true;

    private Vector3 currentRecordedPosition = Vector3.zero;

    private void Awake()
    {
        playerControls = new PlayerControls();
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

        jumpDuration = clockCounter.GetBeatInterval();

        int currentRound = GameManager.Instance.GetCurrentRound();
        recordingDataSequencer.currentRoundRecData = recordingDataSequencer.recordingDataOfRounds[currentRound];
        recordingDataSO = recordingDataSequencer.currentRoundRecData;

        currentRecordedPosition = transform.position;
        rootPosition = currentRecordedPosition;
    }

    void Update()
    {
        rootPosition = transform.position;

        if (isActive)
        {
            //===Moving
            Vector2 moveInput = playerControls.GenericInput.RThumb.ReadValue<Vector2>();
            isMoving = (moveInput.magnitude > 0.001) ? true : false;
            float xInput = moveInput.x;
            float zInput = moveInput.y;
            if (isMoving)
            {
                switch (viewSwitcher.CurrentView)  // Access the current view from ViewSwitcher
                {
                    case ViewSwitcher.ViewSwitch.front:
                        rootPosition = new Vector3(-xInput, currentRecordedPosition.y, -zInput);
                        break;
                    case ViewSwitcher.ViewSwitch.top:
                        rootPosition = new Vector3(-xInput, currentRecordedPosition.y, -zInput);
                        break;
                    case ViewSwitcher.ViewSwitch.left:
                        rootPosition = new Vector3(-zInput, currentRecordedPosition.y, xInput);
                        break;
                    case ViewSwitcher.ViewSwitch.right:
                        rootPosition = new Vector3(zInput, currentRecordedPosition.y, -xInput);
                        break;
                    default:
                        viewSwitcher.CurrentView = ViewSwitcher.ViewSwitch.front; // Set default view to front if the current view is not recognized
                        break;
                }
            }
            //===Jumping
            grounded = (transform.position.y == 0) ? true : false;

            if (playerControls.GenericInput.AButton.IsPressed() && grounded && !isJumping)
            {
                isJumping = true;
                jumpTime = 0;
            }
            if (isJumping && jumpTime < jumpDuration)
            {
                jumpTime += Time.deltaTime;
                float normalizedTime = jumpTime / jumpDuration; // Scale time to the duration
                float yPosition = Mathf.Sin(normalizedTime * Mathf.PI) * jumpHeight; // Peak at jumpHeight

                rootPosition = new Vector3(currentRecordedPosition.x, yPosition, currentRecordedPosition.z);
            }
            if (isJumping && jumpTime >= jumpDuration)
            {
                rootPosition = new Vector3(currentRecordedPosition.x, 0, currentRecordedPosition.z);
                grounded = true;
                jumpTime = 0;
                isJumping = false;
            }
        }
        
        isRecording = true ? (isActive && (isJumping || isMoving || isTurning)) : false;

        transform.position = rootPosition;
    }
    
    /*private void Jump()
    {
        grounded = false;
        jumpTime = 0;
        StartCoroutine(JumpCoroutine());
    }
    private IEnumerator JumpCoroutine()
    {
        while (jumpTime <= jumpDuration)
        {
            jumpTime += Time.deltaTime;

            float normalizedTime = jumpTime / jumpDuration; // Scale time to the duration
            float yPosition = Mathf.Sin(normalizedTime * Mathf.PI) * jumpHeight; // Peak at jumpHeight

            rootPosition = new Vector3(rootPosition.x, yPosition, rootPosition.z);
            transform.position = rootPosition;
            yield return null;
        }
        //securing the conclusion of the animation:
        jumpTime = jumpDuration;
        rootPosition = new Vector3(rootPosition.x, 0, rootPosition.z);
        grounded = true;
        isJumping = false;
    }*/

    public Vector3 GetRootPosition()
    {
        return rootPosition;
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

        if (!isRecording)
        {
            // Tween the object's position to the next recorded position
            transform.DOMove(tweenTargetPosition, tweenDuration).SetEase(Ease.Linear);
        }

        // This tween is moving a Vector3 that can be used for axis not controlled by player input (depending on view switching)
        // Note: it is not actually moving the object, only the Vector3 (currentRecordedPosition) that can be referred to in the Update method
        float xTarget = tweenTargetPosition.x;
        float yTarget = tweenTargetPosition.y;
        float zTarget = tweenTargetPosition.z;
        DOTween.To(() => currentRecordedPosition, x => currentRecordedPosition = x, new Vector3(xTarget, yTarget, zTarget), tweenDuration).SetEase(Ease.Linear);

    }
}
