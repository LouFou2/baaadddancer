using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManipulator : MonoBehaviour
{
    [SerializeField] private Material screenRenderTextureMat;
    private PlayerControls playerControls;
    [SerializeField] private MeshRenderer meshRenderer;

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
        Vector2 thumbL = playerControls.GenericInput.LThumb.ReadValue<Vector2>();
        Vector2 thumbR = playerControls.GenericInput.RThumb.ReadValue<Vector2>();

        float thumbMagL = thumbL.magnitude;
        float thumbMagR = thumbR.magnitude;

        screenRenderTextureMat.SetFloat("_ThumbMagnitudeL", thumbMagL);
        screenRenderTextureMat.SetFloat("_ThumbMagnitudeR", thumbMagR);

        screenRenderTextureMat.SetFloat("_ThumbMagL", thumbMagL);
        screenRenderTextureMat.SetFloat("_ThumbMagR", thumbMagR);

        screenRenderTextureMat.SetVector("_ThumbPosL", new Vector4(thumbL.x, thumbL.y, 0, 0));
        screenRenderTextureMat.SetVector("_ThumbPosR", new Vector4(thumbR.x, thumbR.y, 0, 0));

        screenRenderTextureMat.SetVector("_ThumbInL", new Vector4(thumbL.x, thumbL.y, 0, 0));
        screenRenderTextureMat.SetVector("_ThumbInR", new Vector4(thumbR.x, thumbR.y, 0, 0));
    }
}
