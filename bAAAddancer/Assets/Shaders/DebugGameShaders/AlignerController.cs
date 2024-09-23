using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignerController : MonoBehaviour
{
    private PlayerControls playerControls;
    [SerializeField] private Material alignShaderMat;

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

    private void Update()
    {
        Vector2 controlInputL = playerControls.DanceControls.MoveR.ReadValue<Vector2>();
        float shaderVectorYL = Mathf.InverseLerp(-1, 1, controlInputL.y);

        Vector2 controlInputR = playerControls.DanceControls.MoveL.ReadValue<Vector2>();
        float shaderVectorXR = Mathf.InverseLerp(-1, 1, controlInputR.x);

        Vector2 shaderVector = new Vector2(shaderVectorYL, shaderVectorXR);

        alignShaderMat.SetVector("_ControllerXY", shaderVector);
    }
}
