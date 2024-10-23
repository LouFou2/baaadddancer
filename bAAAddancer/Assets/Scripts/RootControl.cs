using System.Collections;
using UnityEngine;
using DG.Tweening;

public class RootControl : MonoBehaviour
{
    private ClockCounter clockCounter;
    private PlayerControls playerControls;
    private Vector3 rootPosition = Vector3.zero;

    //Data SO's:
    [SerializeField] private RoundsRecData recordingDataOfRounds;
    [SerializeField] private RecordingData recordingDataSO;

    [SerializeField] private bool grounded = true;
    [SerializeField] private float jumpHeight = 0.5f;
    [SerializeField] private float jumpTime = 0f;
    [SerializeField] private float jumpDuration = 0f;

    [SerializeField] private bool useRecordedPositions = true;
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
        jumpDuration = clockCounter.GetBeatInterval();

        recordingDataSO = recordingDataOfRounds.currentRoundRecData;
    }

    void Update()
    {
        if (transform.position.y > 0)
        {
            grounded = false;
        }

        if (playerControls.GenericInput.AButton.IsPressed() && grounded)
        {
            Jump();
        }

        rootPosition = transform.position;
        Debug.Log(rootPosition);
    }
    
    private void Jump()
    {
        grounded = false;
        //Animate a jump with maths
        jumpTime = 0;
        StartCoroutine(JumpCoroutine());
    }
    private IEnumerator JumpCoroutine()
    {
        while (jumpTime <= jumpDuration)
        {
            jumpTime += Time.deltaTime;

            float normalizedTime = jumpTime / jumpDuration; // Scale time to the duration
            float yPosition = jumpHeight * Mathf.Sin(normalizedTime * Mathf.PI); // Peak at jumpHeight

            transform.position = new Vector3(transform.position.x, yPosition, transform.position.z);
            yield return null;
        }
        //securing the conclusion of the animation:
        jumpTime = jumpDuration;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        grounded = true;
    }

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

        if (transform.position != recordingDataSO.recordedPositions[currentPositionIndex])
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
