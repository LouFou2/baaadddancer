using UnityEngine;

public class AlignerController : MonoBehaviour
{
    private PlayerControls playerControls;
    private DebugManager debugManager;

    [SerializeField] private Material alignShaderMat;
    [SerializeField] [Range(-0.99f, 0.99f)] private float randomAlignValueX;
    [SerializeField] [Range(-0.99f, 0.99f)] private float randomAlignValueY;

    bool alignmentLocked = false;

    [SerializeField] private GameObject endObject;

    private float totalDiscrepency;

    private void Awake()
    {
        playerControls = new PlayerControls();

        randomAlignValueX = Random.Range(-0.99f, 0.99f);
        randomAlignValueY = Random.Range(-0.99f, 0.99f);

        endObject.SetActive(false);

        debugManager = FindObjectOfType<DebugManager>();
    }
    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        if (!alignmentLocked)
        {
            // we creat a moving "target" for x and y values
            float adjustedValueX = randomAlignValueX + Random.Range(-0.001f, 0.001f);
            randomAlignValueX = Mathf.Clamp(adjustedValueX, -0.99f, 0.99f);

            float adjustedValueY = randomAlignValueY + Random.Range(-0.001f, 0.001f);
            randomAlignValueY = Mathf.Clamp(adjustedValueY, -0.99f, 0.99f);

            //then, we measure input from controller thumbsticks
            Vector2 controlInputL = playerControls.GenericInput.LThumb.ReadValue<Vector2>();
            float shaderVectorXL = controlInputL.x;
            //float shaderVectorXL = Mathf.InverseLerp(-1, 1, controlInputL.x); //this is only if the input value needs to be normalized

            Vector2 controlInputR = playerControls.GenericInput.RThumb.ReadValue<Vector2>();
            float shaderVectorYR = controlInputR.y;
            //float shaderVectorYR = Mathf.InverseLerp(-1, 1, controlInputR.y); //this is only if the input value needs to be normalized

            Vector2 controllersPosition = new Vector2(shaderVectorXL, shaderVectorYR);
            Vector2 targetPosition = new Vector2(randomAlignValueX, randomAlignValueY);

            //then we measure the difference between the x and y controller input values
            //and the "target" values
            float differenceX = (randomAlignValueX - shaderVectorXL);
            float differenceY = (randomAlignValueY - shaderVectorYR);

            //float clampedX = Mathf.Clamp(differenceX, -1, 1);
            //float clampedY = Mathf.Clamp(differenceY, -1, 1);

            float invLerpX = Mathf.InverseLerp(-1, 1, differenceX) ;
            float invLerpY = Mathf.InverseLerp(-1, 1, differenceY) ;

            float lerpX = Mathf.Lerp(-1, 1, invLerpX);
            float lerpY = Mathf.Lerp(-1, 1, invLerpY);

            Vector2 shaderVector = new Vector2(lerpX, lerpY);

            totalDiscrepency = (Mathf.Abs(lerpX) + Mathf.Abs(lerpY)) * 0.5f;

            alignShaderMat.SetVector("_ControllerXY", shaderVector);
        }
        
        if (playerControls.GenericInput.LTrigger.IsPressed())
        {
            alignmentLocked = true;
            endObject.SetActive(true);
        }
        if (playerControls.GenericInput.RTrigger.IsPressed() && alignmentLocked == true)
        {
            alignmentLocked = false;
            endObject.SetActive(false);
        }

        if (alignmentLocked && playerControls.GenericInput.AButton.IsPressed()) 
        {
            debugManager.EndAlignment(totalDiscrepency);
        }
    }

    public void UnlockAlignment()
    {
        alignmentLocked = false;
        endObject.SetActive(false);
    }
}
