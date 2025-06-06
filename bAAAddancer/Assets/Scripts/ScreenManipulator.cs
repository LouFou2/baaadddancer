using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManipulator : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private Mesh[] glitchMeshes;
    [SerializeField] private int meshIndex = 0;
    [SerializeField] private Material screenRenderTextureMat;
    private PlayerControls playerControls;
    [SerializeField] private MeshRenderer meshRenderer;

    private Vector2 thumbL = Vector2.zero;
    private Vector2 thumbR = Vector2.zero;

    private float thumbMagL = 0f;
    private float thumbMagR = 0f;

    private bool isDemonReveal = false; // *** MOST IMORTANT BOOL IN THIS SCRIPT!

    private void Awake()
    {
        playerControls = new PlayerControls();
    }
    private void OnEnable()
    {
        playerControls.Enable();
        ClockCounter.On_Beat_Trigger += OnBeatHandler; // Subscribe to the beat trigger event

        // ** STUPIDLY THIS IS GETTING IT FROM THE AUDIO MANAGER, IT COULD JUST HANDLE IT INTERNALLY
        AudioManager.On_DemonReveal += DemonRevealHandler; //the audioManager will trigger this on the correct beat of the track for demon reveal
    }
    private void OnDisable()
    {
        playerControls.Disable();
        ClockCounter.On_Beat_Trigger -= OnBeatHandler; // Unsubscribe to the beat trigger event

        AudioManager.On_DemonReveal -= DemonRevealHandler;
    }
    private void Start()
    {
        thumbL = Vector2.zero;
        thumbR = Vector2.zero;

        thumbMagL = thumbL.magnitude;
        thumbMagR = thumbR.magnitude;

        screenRenderTextureMat.SetFloat("_ThumbMagnitudeL", thumbMagL);
        screenRenderTextureMat.SetFloat("_ThumbMagnitudeR", thumbMagR);

        screenRenderTextureMat.SetFloat("_ThumbMagL", thumbMagL);
        screenRenderTextureMat.SetFloat("_ThumbMagR", thumbMagR);

        screenRenderTextureMat.SetVector("_ThumbPosL", new Vector4(thumbL.x, thumbL.y, 0, 0));
        screenRenderTextureMat.SetVector("_ThumbPosR", new Vector4(thumbR.x, thumbR.y, 0, 0));

        screenRenderTextureMat.SetVector("_ThumbInL", new Vector4(thumbL.x, thumbL.y, 0, 0));
        screenRenderTextureMat.SetVector("_ThumbInR", new Vector4(thumbR.x, thumbR.y, 0, 0));
    }

    void Update()
    {
        if (!isDemonReveal)
        {
            return;
        }

        thumbL = playerControls.GenericInput.LThumb.ReadValue<Vector2>();
        thumbR = playerControls.GenericInput.RThumb.ReadValue<Vector2>();

        thumbMagL = thumbL.magnitude;
        thumbMagR = thumbR.magnitude;

        screenRenderTextureMat.SetFloat("_ThumbMagnitudeL", thumbMagL);
        screenRenderTextureMat.SetFloat("_ThumbMagnitudeR", thumbMagR);

        screenRenderTextureMat.SetFloat("_ThumbMagL", thumbMagL);
        screenRenderTextureMat.SetFloat("_ThumbMagR", thumbMagR);

        screenRenderTextureMat.SetVector("_ThumbPosL", new Vector4(thumbL.x, thumbL.y, 0, 0));
        screenRenderTextureMat.SetVector("_ThumbPosR", new Vector4(thumbR.x, thumbR.y, 0, 0));

        screenRenderTextureMat.SetVector("_ThumbInL", new Vector4(thumbL.x, thumbL.y, 0, 0));
        screenRenderTextureMat.SetVector("_ThumbInR", new Vector4(thumbR.x, thumbR.y, 0, 0));
    }

    void OnBeatHandler()
    {
        meshIndex++;
        if (meshIndex >= glitchMeshes.Length)
        {
            meshIndex = 0; // this just keeps it looping
        }
        meshFilter.mesh = glitchMeshes[meshIndex];
    }

    public Vector2 GetThumbVectorL()
    {
        return thumbL;
    }
    public Vector2 GetThumbVectorR()
    {
        return thumbR;
    }
    public float GetThumbMagL()
    {
        return thumbMagL;
    }
    public float GetThumbMagR()
    {
        return thumbMagR;
    }

    // ** STUPIDLY THIS IS GETTING IT FROM THE AUDIO MANAGER, IT COULD JUST HANDLE IT INTERNALLY
    void DemonRevealHandler()
    {
        isDemonReveal = true;
    }
}
