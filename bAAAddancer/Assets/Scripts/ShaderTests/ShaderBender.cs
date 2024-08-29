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

    private float timer = 0f;
    
    /*[System.Serializable]
    public class BoneData 
    {
        [SerializeField] public GameObject bone;
        [SerializeField] public Rigidbody boneRb;
    }
    [SerializeField] private BoneData[] bonesData;*/

    [SerializeField] private GameObject headBone;
    [SerializeField] private GameObject hand_L_Bone;
    [SerializeField] private GameObject hand_R_Bone;
    [SerializeField] private GameObject foot_L_Bone;
    [SerializeField] private GameObject foot_R_Bone;

    [SerializeField] private GameObject headControlTarget;
    [SerializeField] private GameObject hand_L_ControlTarget;
    [SerializeField] private GameObject hand_R_ControlTarget;
    [SerializeField] private GameObject foot_L_ControlTarget;
    [SerializeField] private GameObject foot_R_ControlTarget;

    [SerializeField] private GameObject headBoneFollower1;
    [SerializeField] private GameObject hand_L_BoneFollower1;
    [SerializeField] private GameObject hand_R_BoneFollower1;
    [SerializeField] private GameObject foot_L_BoneFollower1;
    [SerializeField] private GameObject foot_R_BoneFollower1;

    private Vector3 headFollowerDirection;
    private Vector3 hand_L_FollowerDirection;
    private Vector3 hand_R_FollowerDirection;
    private Vector3 foot_L_FollowerDirection;
    private Vector3 foot_R_FollowerDirection;

    private float isHead = 0;
    private float isHands = 0;
    private float isFeet = 0;

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
            UpdatePositions();
        }
    }

    void UpdatePositions()
    {
        // the bone positions
        Vector3 headPos = headBone.transform.position;
        Vector3 handLPos = hand_L_Bone.transform.position;
        Vector3 handRPos = hand_R_Bone.transform.position;
        Vector3 footLPos = foot_L_Bone.transform.position;
        Vector3 footRPos = foot_R_Bone.transform.position;

        // the control target positions
        Vector3 headTargetPos = headControlTarget.transform.position;
        Vector3 handLTargetPos = hand_L_ControlTarget.transform.position;
        Vector3 handRTargetPos = hand_R_ControlTarget.transform.position;
        Vector3 footLTargetPos = foot_L_ControlTarget.transform.position;
        Vector3 footRTargetPos = foot_R_ControlTarget.transform.position;

        // the follower positions
        Vector3 head_FollowerPos = headBoneFollower1.transform.position;
        Vector3 handL_FollowerPos = hand_L_BoneFollower1.transform.position;
        Vector3 handR_FollowerPos = hand_R_BoneFollower1.transform.position;
        Vector3 footL_FollowerPos = foot_L_BoneFollower1.transform.position;
        Vector3 footR_FollowerPos = foot_R_BoneFollower1.transform.position;

        headFollowerDirection = CalculateFollowerDirection(headTargetPos, head_FollowerPos);
        hand_L_FollowerDirection = CalculateFollowerDirection(handLTargetPos, handL_FollowerPos);
        hand_R_FollowerDirection = CalculateFollowerDirection(handRTargetPos, handR_FollowerPos);
        foot_L_FollowerDirection = CalculateFollowerDirection(footLTargetPos, footL_FollowerPos);
        foot_R_FollowerDirection = CalculateFollowerDirection(footRTargetPos, footR_FollowerPos);
        
        

        // set bone positions in material properties
        material.SetVector("_HeadBonePosition", headPos);
        material.SetVector("_Hand_L_BonePosition", handLPos);
        material.SetVector("_Hand_R_BonePosition", handRPos);
        material.SetVector("_Foot_L_BonePosition", footLPos);
        material.SetVector("_Foot_R_BonePosition", footRPos);

        // set follower directions in material properties
        material.SetVector("_HeadFollowerDirection", headFollowerDirection);
        material.SetVector("_Hand_L_FollowerDirection", hand_L_FollowerDirection);
        material.SetVector("_Hand_R_FollowerDirection", hand_R_FollowerDirection);
        material.SetVector("_Foot_L_FollowerDirection", foot_L_FollowerDirection);
        material.SetVector("_Foot_R_FollowerDirection", foot_R_FollowerDirection);


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

        switch (controlObjectIndex)
        {
            case (0):
                break;
            case (1):
                isFeet = 1;
                break;
            case (2):
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
    }
    

}
