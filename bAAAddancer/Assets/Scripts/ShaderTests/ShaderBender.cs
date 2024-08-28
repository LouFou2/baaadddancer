using UnityEngine;

public class ShaderBender : MonoBehaviour
{
    public Material material; // Assign your material in the inspector

    private PlayerControls playerControls;
    private int controlObjectIndex = 0;
    private bool switchControlObjectPrevious;
    private bool switchControlObjectNext;

    private float timer = 0f;

    [SerializeField] private GameObject headBone;
    [SerializeField] private GameObject hand_L_Bone;
    [SerializeField] private GameObject hand_R_Bone;
    [SerializeField] private GameObject foot_L_Bone;
    [SerializeField] private GameObject foot_R_Bone;

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
        Vector3 headPos = headBone.transform.position;
        Vector3 handL = hand_L_Bone.transform.position;
        Vector3 handR = hand_R_Bone.transform.position;
        Vector3 footL = foot_L_Bone.transform.position;
        Vector3 footR = foot_R_Bone.transform.position;

        HandleInput();

        material.SetVector("_HeadBonePosition", headPos);
        material.SetVector("_Hand_L_BonePosition", handL);
        material.SetVector("_Hand_R_BonePosition", handR);
        material.SetVector("_Foot_L_BonePosition", footL);
        material.SetVector("_Foot_R_BonePosition", footR);
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
