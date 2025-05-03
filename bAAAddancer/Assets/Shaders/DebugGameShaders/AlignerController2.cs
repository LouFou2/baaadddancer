using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignerController2 : MonoBehaviour
{
    private PlayerControls playerControls;

    [SerializeField] private UIControlsAnimManager uiAnimsManager;
    [SerializeField] private Material alignerImageMAT; // assign in inspector

    private Vector2 randomAlignVectorL;
    private Vector2 randomAlignVectorR;

    private Vector2 storedInputX = Vector2.zero;
    private Vector2 storedInputY = Vector2.zero;

    private float finalAlignedAmount;

    private float time;

    public bool isLocked = false;
    public bool alignerRunning = false;

    public static event System.Action On_AlignerComplete; // subscribed to by the DebugUI_Manager, and the Curse Manager

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

    void Start()
    {
        // Set L Thumb random target Vector on circumference
        float angleL = Random.Range(0f, Mathf.PI * 2);
        randomAlignVectorL = new Vector2(Mathf.Cos(angleL), Mathf.Sin(angleL)); // (-1, -1) to (1, 1) range

        //***this is just for debugging. it is remapped 0-1 for input value because we use UV coordinates in the shader/material
        Vector2 remappedTargetL = new Vector2(Mathf.InverseLerp(-1f, 1f, randomAlignVectorL.x), Mathf.InverseLerp(-1f, 1f, randomAlignVectorL.y));
        alignerImageMAT.SetVector("_TempDebugTargetL", remappedTargetL);
        //***

        // Set R Thumb random target Vector on circumference
        float angleR = Random.Range(0f, Mathf.PI * 2);
        randomAlignVectorR = new Vector2(Mathf.Cos(angleR), Mathf.Sin(angleR)); // (-1, -1) to (1, 1) range

        //***just for debugging.
        Vector2 remappedTargetR = new Vector2(Mathf.InverseLerp(-1f, 1f, randomAlignVectorR.x), Mathf.InverseLerp(-1f, 1f, randomAlignVectorR.y));
        alignerImageMAT.SetVector("_TempDebugTargetR", remappedTargetR);
        //***
    }

    void Update()
    {
        if (!alignerRunning)
        {
            return;
        }

        time += Time.deltaTime;

        // Input
        if (!isLocked) // it will store the last input(or defaults) when the aligners get locked  
        {
            storedInputX = playerControls.GenericInput.LThumb.ReadValue<Vector2>();
            storedInputY = playerControls.GenericInput.RThumb.ReadValue<Vector2>();
        }
        Vector2 controlInputL = storedInputX;
        Vector2 controlInputR = storedInputY;

        float thumbAmountL = controlInputL.magnitude;
        float thumbAmountR = controlInputR.magnitude; // this returns 0-1

        if (playerControls.GenericInput.RTrigger.triggered)
        {
            isLocked = !isLocked;
        }

        // Interference / random movement
        // 1. the amplitude = the discrepency between the random mystery value and the input
        // 2. the frequency = faster the closer it gets to the mystery value

        Vector2 randomDirX = randomAlignVectorL.normalized; // "X" as in the left thumb controlling the left-right bar
        Vector2 inputDirX = controlInputL.normalized;
        

        Vector2 randomDirY = randomAlignVectorR.normalized; // "Y" as in the right thumb controlling the up-down bar
        Vector2 inputDirY = controlInputR.normalized;

        float alignmentX = Vector2.Dot(randomDirX, inputDirX);
        float alignmentY = Vector2.Dot(randomDirY, inputDirY);

        float remappedX = Mathf.InverseLerp(-1f, 1f, alignmentX); // 0 = opposite, 1 = aligned
        float remappedY = Mathf.InverseLerp(-1f, 1f, alignmentY);

        float factorThumbAmountX = remappedX * thumbAmountL; // this uses the amount the thumbstick is pushed to the edge as a factor
        float factorThumbAmountY = remappedY * thumbAmountR; // so no pushing will 0 out, fully on edge will be * 1

        float ampX = 1 - factorThumbAmountX; // need this because 1 is "max discrepency"
        float ampY = 1 - factorThumbAmountY;

        //*** CAN REMOVE *** older "distance"/discrepency calculation:
        /*float ampX = Vector2.Distance(randomAlignVectorL, controlInputL) * 0.5f; // we half this range because the range is 0-2
        float ampY = Vector2.Distance(randomAlignVectorR, controlInputR) * 0.5f; // e.g. (0,-1) - (0, 1) = (0, -2) */

        float freqX = Mathf.Lerp(10f, 0.5f, ampX); // see how the speed will be faster the closer we are to the mystery value (the smaller the distance)
        float freqY = Mathf.Lerp(20f, 0.5f, ampY);

        float xBarSineValue = Mathf.Sin(time * freqX) * ampX;
        float yBarSineValue = Mathf.Sin(time * freqY) * ampY;

        // Remap to 0-1 values
        float remapX = Mathf.InverseLerp(-1, 1, xBarSineValue);
        float remapY = Mathf.InverseLerp(-1, 1, yBarSineValue);

        // Set Shader Material Values
        alignerImageMAT.SetFloat("_BarPosX", remapX);
        alignerImageMAT.SetFloat("_BarPosY", remapY);

        float remapThumbL_X = Mathf.InverseLerp(-1, 1, controlInputL.x); 
        float remapThumbL_Y = Mathf.InverseLerp(-1, 1, controlInputL.y);
        float remapThumbR_X = Mathf.InverseLerp(-1, 1, controlInputR.x);
        float remapThumbR_Y = Mathf.InverseLerp(-1, 1, controlInputR.y);

        Vector2 remapThumbL = new Vector2(remapThumbL_X, remapThumbL_Y);
        Vector2 remapThumbR = new Vector2(remapThumbR_X, remapThumbR_Y);

        alignerImageMAT.SetVector("_ThumbInputX", remapThumbL);
        alignerImageMAT.SetVector("_ThumbInputY", remapThumbR);

        // Calculate Final Curse Amount (Debugged)
        finalAlignedAmount = (ampX + ampY) * 0.5f;  // can just use the amp calculations (as this is based on the difference between target and input)

        // we check for Exit at the end of calculations
        if (playerControls.GenericInput.YButton.triggered && isLocked) // can only exit if the aligner is locked
        {
            EndAligner();
        }
    }
    public void StartAligner() // called from the DebugUI_Manager
    {
        alignerRunning = true;
        isLocked = false;
        uiAnimsManager.StartUIAnims();
    }
    public void EndAligner() 
    {
        alignerRunning = false;
        On_AlignerComplete?.Invoke(); // subscribed to by the DebugUI_Manager
    }

    public float GetFinalAlignedAmount()
    {
        return finalAlignedAmount;
    }
}
