using UnityEngine;

public class ShaderBender : MonoBehaviour
{
    public Material material; // Assign your material in the inspector

    [SerializeField] private float updateInterval = 0.1f; // Time in seconds between updates
    private float timeSinceLastUpdate = 0f;

    //private float timer = 0f; // this can be used for something else

    private PlayerControls playerControls;
    private int controlObjectIndex = 0;
    private bool switchControlObjectPrevious;
    private bool switchControlObjectNext;

    [SerializeField] private GameObject headBone;
    [SerializeField] private GameObject hand_L_Bone;
    [SerializeField] private GameObject hand_R_Bone;
    [SerializeField] private GameObject foot_L_Bone;
    [SerializeField] private GameObject foot_R_Bone;
    [SerializeField] private GameObject pelvisBone;
    [SerializeField] private GameObject torsoBone;

    [SerializeField] private GameObject headControlTarget;
    [SerializeField] private GameObject hand_L_ControlTarget;
    [SerializeField] private GameObject hand_R_ControlTarget;
    [SerializeField] private GameObject foot_L_ControlTarget;
    [SerializeField] private GameObject foot_R_ControlTarget;
    [SerializeField] private GameObject pelvisControlTarget;
    [SerializeField] private GameObject torsoControlTarget;

    [SerializeField] private GameObject headFollower1;
    [SerializeField] private GameObject hand_L_Follower1;
    [SerializeField] private GameObject hand_R_Follower1;
    [SerializeField] private GameObject foot_L_Follower1;
    [SerializeField] private GameObject foot_R_Follower1;
    [SerializeField] private GameObject pelvisFollower1;
    [SerializeField] private GameObject torsoFollower1;

    private Vector3 headFollowerDirection;
    private Vector3 hand_L_FollowerDirection;
    private Vector3 hand_R_FollowerDirection;
    private Vector3 foot_L_FollowerDirection;
    private Vector3 foot_R_FollowerDirection;
    private Vector3 pelvisFollowerDirection;
    private Vector3 torsoFollowerDirection;

    [SerializeField] private float moveAmountMultiplier = 1;
    private float headMoveAmount;
    private float hand_L_MoveAmount;
    private float hand_R_MoveAmount;
    private float foot_L_MoveAmount;
    private float foot_R_MoveAmount;
    private float pelvisMoveAmount;
    private float torsoMoveAmount;

    private float isHead = 0;
    private float isHands = 0;
    private float isFeet = 0;
    private float isPelvis = 0;
    private float isTorso = 0;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }
    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }

    void Update()
    {
        HandleInput();

        timeSinceLastUpdate += Time.deltaTime;

        if (timeSinceLastUpdate >= updateInterval)
        {
            timeSinceLastUpdate = 0f; // Reset the timer
            UpdateProperties();
        }
    }

    void UpdateProperties()
    {
        // the bone positions
        Vector3 headPos = headBone.transform.position;
        Vector3 handLPos = hand_L_Bone.transform.position;
        Vector3 handRPos = hand_R_Bone.transform.position;
        Vector3 footLPos = foot_L_Bone.transform.position;
        Vector3 footRPos = foot_R_Bone.transform.position;
        Vector3 pelvisPos = pelvisBone.transform.position;
        Vector3 torsoPos = torsoBone.transform.position;

        // the control target positions
        Vector3 headTargetPos = headControlTarget.transform.position;
        Vector3 handLTargetPos = hand_L_ControlTarget.transform.position;
        Vector3 handRTargetPos = hand_R_ControlTarget.transform.position;
        Vector3 footLTargetPos = foot_L_ControlTarget.transform.position;
        Vector3 footRTargetPos = foot_R_ControlTarget.transform.position;
        Vector3 pelvisTargetPos = pelvisControlTarget.transform.position;
        Vector3 torsoTargetPos = torsoControlTarget.transform.position;

        // the follower positions
        Vector3 head_FollowerPos = headFollower1.transform.position;
        Vector3 handL_FollowerPos = hand_L_Follower1.transform.position;
        Vector3 handR_FollowerPos = hand_R_Follower1.transform.position;
        Vector3 footL_FollowerPos = foot_L_Follower1.transform.position;
        Vector3 footR_FollowerPos = foot_R_Follower1.transform.position;
        Vector3 pelvisFollowerPos = pelvisFollower1.transform.position;
        Vector3 torsoFollowerPos = torsoFollower1.transform.position;

        headFollowerDirection = CalculateFollowerDirection(headTargetPos, head_FollowerPos);
        hand_L_FollowerDirection = CalculateFollowerDirection(handLTargetPos, handL_FollowerPos);
        hand_R_FollowerDirection = CalculateFollowerDirection(handRTargetPos, handR_FollowerPos);
        foot_L_FollowerDirection = CalculateFollowerDirection(footLTargetPos, footL_FollowerPos);
        foot_R_FollowerDirection = CalculateFollowerDirection(footRTargetPos, footR_FollowerPos);
        pelvisFollowerDirection = CalculateFollowerDirection(pelvisTargetPos, pelvisFollowerPos);
        torsoFollowerDirection = CalculateFollowerDirection(torsoTargetPos, torsoFollowerPos);

        // measuring move amount (distance between control object and the damped follow object)
        headMoveAmount = Vector3.Distance(headTargetPos, head_FollowerPos) * moveAmountMultiplier;
        hand_L_MoveAmount = Vector3.Distance(handLTargetPos, handL_FollowerPos) * moveAmountMultiplier;
        hand_R_MoveAmount = Vector3.Distance(handRTargetPos, handR_FollowerPos) * moveAmountMultiplier;
        foot_L_MoveAmount = Vector3.Distance(footLTargetPos, footL_FollowerPos) * moveAmountMultiplier;
        foot_R_MoveAmount = Vector3.Distance(footRTargetPos, footR_FollowerPos) * moveAmountMultiplier;
        pelvisMoveAmount = Vector3.Distance(pelvisTargetPos, pelvisFollowerPos) * moveAmountMultiplier;
        torsoMoveAmount = Vector3.Distance(torsoTargetPos, torsoFollowerPos) * moveAmountMultiplier;

        // set bone positions in material properties
        material.SetVector("_HeadBonePosition", headPos);
        material.SetVector("_Hand_L_BonePosition", handLPos);
        material.SetVector("_Hand_R_BonePosition", handRPos);
        material.SetVector("_Foot_L_BonePosition", footLPos);
        material.SetVector("_Foot_R_BonePosition", footRPos);
        material.SetVector("_PelvisBonePosition", pelvisPos);
        material.SetVector("_TorsoBonePosition", torsoPos);

        // set follower directions in material properties
        material.SetVector("_HeadFollowerDirection", headFollowerDirection);
        material.SetVector("_Hand_L_FollowerDirection", hand_L_FollowerDirection);
        material.SetVector("_Hand_R_FollowerDirection", hand_R_FollowerDirection);
        material.SetVector("_Foot_L_FollowerDirection", foot_L_FollowerDirection);
        material.SetVector("_Foot_R_FollowerDirection", foot_R_FollowerDirection);
        material.SetVector("_PelvisFollowerDirection", pelvisFollowerDirection);
        material.SetVector("_TorsoFollowerDirection", torsoFollowerDirection);

        // set moveAmounts in material properties
        material.SetFloat("_HeadMoveAmount", headMoveAmount);
        material.SetFloat("_Hand_L_MoveAmount", hand_L_MoveAmount);
        material.SetFloat("_Hand_R_MoveAmount", hand_R_MoveAmount);
        material.SetFloat("_Foot_L_MoveAmount", foot_L_MoveAmount);
        material.SetFloat("_Foot_R_MoveAmount", foot_R_MoveAmount);
        material.SetFloat("_PelvisMoveAmount", pelvisMoveAmount);
        material.SetFloat("_TorsoMoveAmount", torsoMoveAmount);

    }

    Vector3 CalculateFollowerDirection(Vector3 bonePosition, Vector3 followerPosition) 
    {
        //calculating directions:
        // NNNB: we MUST ensure they are NOT the same position as the bones they follow:
        /*
         * When the followerPosition and bonePosition are the same, their difference results in a zero vector (float3(0, 0, 0)).
         * Normalizing a zero vector is mathematically undefined because the length (magnitude) of a zero vector is zero, 
         * and dividing by zero is not possible.
        */
        Vector3 direction = followerPosition - bonePosition;
        Vector3 tailDirection = direction.magnitude > 0 ? direction.normalized : Vector3.zero;
        return tailDirection;
    }

    void HandleInput()
    {
        // first we determine the current active control objects
        int nextIndex = ((controlObjectIndex + 1) > 4) ? 0 : controlObjectIndex + 1;       // there are 5 pairs of control objects
        int previousIndex = ((controlObjectIndex - 1) < 0) ? 4 : controlObjectIndex - 1;    // also we make sure it loops 0-5                                               
        // the controlObjects gets switched by the bumpers
        switchControlObjectPrevious = playerControls.GenericInput.LBumper.triggered;
        switchControlObjectNext = playerControls.GenericInput.RBumper.triggered;
        if (switchControlObjectPrevious) controlObjectIndex = previousIndex;
        if (switchControlObjectNext) controlObjectIndex = nextIndex;

        //reset all bools
        isHead = 0;
        isHands = 0;
        isFeet = 0;
        isPelvis = 0;
        isTorso = 0;

        switch (controlObjectIndex)
        {
            case (0):
                isPelvis = 1;
                break;
            case (1):
                isFeet = 1;
                break;
            case (2):
                isTorso = 1;
                break;
            case (3):
                isHands = 1;
                break;
            case (4):
                isHead = 1;
                break;
        }

        material.SetFloat("_IsHead", isHead);
        material.SetFloat("_IsHands", isHands);
        material.SetFloat("_IsFeet", isFeet);
        material.SetFloat("_IsPelvis", isPelvis);
        material.SetFloat("_IsTorso", isTorso);
    }
    

}
