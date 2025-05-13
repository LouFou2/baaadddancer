using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManipulator : MonoBehaviour
{
    [SerializeField] private Material screenRenderTextMat;
    private PlayerControls playerControls;

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
        
    }

    void Update()
    {
        Vector2 thumbL = playerControls.GenericInput.LThumb.ReadValue<Vector2>();
        Vector2 thumbR = playerControls.GenericInput.RThumb.ReadValue<Vector2>();

        float thumbMagL = thumbL.magnitude;
        float thumbMagR = thumbR.magnitude;

        screenRenderTextMat.SetFloat("_ThumbMagnitudeL", thumbMagL);
        screenRenderTextMat.SetFloat("_ThumbMagnitudeR", thumbMagR);

        screenRenderTextMat.SetVector("_ThumbPosL", thumbL);
        screenRenderTextMat.SetVector("_ThumbPosR", thumbR);
    }
}
