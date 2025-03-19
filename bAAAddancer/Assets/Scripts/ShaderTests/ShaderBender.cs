using UnityEngine;

public class ShaderBender : MonoBehaviour
{
    public Material[] materials; // Assign your material in the inspector

    [SerializeField] private CharacterData charData;

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

    [SerializeField] [Range(0, 1)] private float cursedness = 0;
    [SerializeField] private float moveAmountMultiplier = 1;
    [SerializeField] private float moveAmountMax = 50;
    [SerializeField] private float irridescence = 0;
    [SerializeField] private float irridescenceMax = 0;
    [SerializeField] [Range(0, 1)] private float flatShading = 0;

    //control how intensely the beat "pulses" the glitches
    [SerializeField] [Range(0, 1)] private float beatPulseFactor = 0.5f;

    private float headMoveAmount;
    private float hand_L_MoveAmount;
    private float hand_R_MoveAmount;
    private float foot_L_MoveAmount;
    private float foot_R_MoveAmount;
    private float pelvisMoveAmount;
    private float torsoMoveAmount;

    private float isHead = 1;
    private float isHands = 1;
    private float isFeet = 1;
    private float isPelvis = 1;
    private float isTorso = 1;

    //timing parameters, to use with "beat" timing
    private int q_BeatCount = -1;
    private float beatPulse = 0;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }
    private void OnEnable()
    {
        playerControls.Enable();
        ClockCounter.On_Q_Beat_Trigger += On_Q_BeatHandler;
    }
    private void OnDisable()
    {
        playerControls.Disable();
        ClockCounter.On_Q_Beat_Trigger -= On_Q_BeatHandler;
    }
    void On_Q_BeatHandler()
    {
        q_BeatCount++;
        if (q_BeatCount == 4)
        {
            q_BeatCount = 0;
        }
        beatPulse = ((4 - q_BeatCount) * 0.25f); // count is 0-3. thus at 0 it is  4 * 0.25 = 1, and at count 3 is 0.25.
        ProcessSpectrumData();
        HandleCursedness();
        UpdateProperties();
    }
    void ProcessSpectrumData()
    {
        //** trying something here: use frequency data to control glitches on different parts of the body
        isPelvis = AudioFrequalizer.freqBand5[0];
        isTorso = AudioFrequalizer.freqBand5[1];
        isFeet = AudioFrequalizer.freqBand5[2];
        isHands = AudioFrequalizer.freqBand5[3];
        isHead = AudioFrequalizer.freqBand5[4];
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
        foreach (Material material in materials)
        {
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

            // shading
            material.SetFloat("_SmoothShading", 1 - flatShading);
            material.SetFloat("_IrridescenceBlend", irridescence);

            //=== was previously switched between, depending on which controls where active
            material.SetFloat("_IsHead", isHead);
            material.SetFloat("_IsHands", isHands);
            material.SetFloat("_IsFeet", isFeet);
            material.SetFloat("_IsPelvis", isPelvis);
            material.SetFloat("_IsTorso", isTorso);
        }
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

        foreach (Material material in materials)
        {
            material.SetFloat("_IsHead", isHead);
            material.SetFloat("_IsHands", isHands);
            material.SetFloat("_IsFeet", isFeet);
            material.SetFloat("_IsPelvis", isPelvis);
            material.SetFloat("_IsTorso", isTorso);
        }
 
    }
    
    void HandleCursedness()
    {
        cursedness = Mathf.Clamp(charData.infectionLevel, 0, 1);

        float beatPulseCursedness = cursedness * beatPulse * beatPulseFactor; // makes the cursed amount "pulse" with the beat

        //moveAmountMultiplier = Mathf.Lerp(0, moveAmountMax, cursedness); // the old one
        moveAmountMultiplier = Mathf.Lerp(0, moveAmountMax, beatPulseCursedness); // using the beat to "pulse" it
        flatShading = cursedness;
        irridescence = Mathf.Lerp(0, irridescenceMax, cursedness);
    }
}
