using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignerController2 : MonoBehaviour
{
    private PlayerControls playerControls;

    [SerializeField] private Material alignerImageMAT; // assign in inspector

    private Vector2 randomAlignVectorL;
    private Vector2 randomAlignVectorR;

    private float finalCurseAmount;

    private float time;

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
        randomAlignVectorL = new Vector2(Mathf.Cos(angleL), Mathf.Sin(angleL));
        Vector2 remappedTargetL = new Vector2(Mathf.InverseLerp(-1f, 1f, randomAlignVectorL.x), Mathf.InverseLerp(-1f, 1f, randomAlignVectorL.y));
        alignerImageMAT.SetVector("_TempDebugTargetL", remappedTargetL);

        // Set R Thumb random target Vector on circumference
        float angleR = Random.Range(0f, Mathf.PI * 2);
        randomAlignVectorR = new Vector2(Mathf.Cos(angleR), Mathf.Sin(angleR));
        Vector2 remappedTargetR = new Vector2(Mathf.InverseLerp(-1f, 1f, randomAlignVectorR.x), Mathf.InverseLerp(-1f, 1f, randomAlignVectorR.y));
        alignerImageMAT.SetVector("_TempDebugTargetR", remappedTargetR);
    }

    void Update()
    {
        time += Time.deltaTime;

        // Input
        Vector2 controlInputL = playerControls.GenericInput.LThumb.ReadValue<Vector2>();
        Vector2 controlInputR = playerControls.GenericInput.RThumb.ReadValue<Vector2>();

        // Interference / random movement
        // 1. the amplitude = the discrepency between the random mystery value and the input
        // 2. the frequency = faster the closer it gets to the mystery value
        float ampX = Vector2.Distance(randomAlignVectorL, controlInputL) * 0.5f; // we half this range because the range is 0-2
        float ampY = Vector2.Distance(randomAlignVectorR, controlInputR) * 0.5f; // e.g. (0,-1) - (0, 1) = (0, -2) 

        float freqX = Mathf.Lerp(100f, 0.5f, ampX); // see how the speed will be faster the closer we are to the mystery value (the smaller the distance)
        float freqY = Mathf.Lerp(100f, 0.5f, ampY);

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
        finalCurseAmount = (ampX + ampY) * 0.5f;  // can just use the amp calculations (as this is based on the difference between target and input)

    }
}
