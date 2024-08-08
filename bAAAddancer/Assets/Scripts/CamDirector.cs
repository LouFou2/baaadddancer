using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamDirector : MonoBehaviour
{
    [SerializeField] private Animator camAnimator;
    void Start()
    {
        camAnimator = gameObject.GetComponent<Animator>();
    }
    public void SetCameraState(CameraDirections cam, int speakingChar, int spokenToChar, CameraDirections camDistance, CameraDirections camAngle, CameraDirections camZoom, CameraDirections camShake ) 
    {
        switch (cam)
        {
            case CameraDirections.LongCam:
                camAnimator.SetBool("Longcam", true);
                camAnimator.SetBool("CharCam1", false);
                camAnimator.SetBool("CharCam2", false);
                camAnimator.SetBool("CharCam3", false);
                camAnimator.SetBool("CharCam4", false);
                camAnimator.SetBool("CharCam5", false);
                camAnimator.SetBool("CharCam6", false);
                HandleCamAngle(camAngle);
                break;
            case CameraDirections.SpeakingCharCam:
                HandleSpeakerCam(speakingChar);
                HandleCamDistance(camDistance);
                HandleCamAngle(camAngle);
                HandleCamZoom(camZoom);
                HandleCamShake(camShake);
                break;
            case CameraDirections.SpokenToCharCam:
                HandleSpokenToCam(spokenToChar);
                HandleCamDistance(camDistance);
                HandleCamAngle(camAngle);
                HandleCamZoom(camZoom);
                HandleCamShake(camShake);
                break;
                // Add other cases for different CameraDirections if needed
        }
    }
    
    private void HandleSpeakerCam(int speakingChar) 
    {
        camAnimator.SetBool("Longcam", false);
        camAnimator.SetBool("CharCam1", false);
        camAnimator.SetBool("CharCam2", false);
        camAnimator.SetBool("CharCam3", false);
        camAnimator.SetBool("CharCam4", false);
        camAnimator.SetBool("CharCam5", false);
        camAnimator.SetBool("CharCam6", false);
        switch (speakingChar) 
        {
            case 0:
                camAnimator.SetBool("CharCam1", true);
                break;
            case 1:
                camAnimator.SetBool("CharCam2", true);
                break;
            case 2:
                camAnimator.SetBool("CharCam3", true);
                break;
            case 3:
                camAnimator.SetBool("CharCam4", true);
                break;
            case 4:
                camAnimator.SetBool("CharCam5", true);
                break;
            case 5:
                camAnimator.SetBool("CharCam6", true);
                break;
        }
    }
    private void HandleSpokenToCam(int spokenToChar) 
    {
        camAnimator.SetBool("Longcam", false);
        camAnimator.SetBool("CharCam1", false);
        camAnimator.SetBool("CharCam2", false);
        camAnimator.SetBool("CharCam3", false);
        camAnimator.SetBool("CharCam4", false);
        camAnimator.SetBool("CharCam5", false);
        camAnimator.SetBool("CharCam6", false);

        switch (spokenToChar)
        {
            case 0:
                camAnimator.SetBool("CharCam1", true);
                break;
            case 1:
                camAnimator.SetBool("CharCam2", true);
                break;
            case 2:
                camAnimator.SetBool("CharCam3", true);
                break;
            case 3:
                camAnimator.SetBool("CharCam4", true);
                break;
            case 4:
                camAnimator.SetBool("CharCam5", true);
                break;
            case 5:
                camAnimator.SetBool("CharCam6", true);
                break;
        }
    }
    private void HandleCamDistance(CameraDirections camDistance) 
    {
        camAnimator.SetBool("CamMed", false);
        camAnimator.SetBool("CamClose", false);
        camAnimator.SetBool("CamXClose", false);
        switch (camDistance) 
        {
            case CameraDirections.CamMed:
                camAnimator.SetBool("CamMed", true);
                break;
            case CameraDirections.CamClose:
                camAnimator.SetBool("CamClose", true);
                break;
            case CameraDirections.CamXClose:
                camAnimator.SetBool("CamXClose", true);
                break;
        }
    }
    private void HandleCamAngle(CameraDirections camAngle)
    {
        camAnimator.SetBool("CamAngleHigh", false);
        camAnimator.SetBool("CamAngleLevel", false);
        camAnimator.SetBool("CamAngleLow", false);
        switch (camAngle)
        {
            case CameraDirections.CamAngleHigh:
                camAnimator.SetBool("CamAngleHigh", true);
                break;
            case CameraDirections.CamAngleLevel:
                camAnimator.SetBool("CamAngleLevel", true);
                break;
            case CameraDirections.CamAngleLow:
                camAnimator.SetBool("CamAngleLow", true);
                break;
        }
    }
    private void HandleCamZoom(CameraDirections camZoom) 
    {
        camAnimator.SetBool("NoZoom", false);
        camAnimator.SetBool("ZoomInFast", false);
        camAnimator.SetBool("ZoomInSlow", false);
        camAnimator.SetBool("ZoomOutFast", false);
        camAnimator.SetBool("ZoomOutSlow", false);
        switch (camZoom)
        {
            case CameraDirections.NoZoom:
                camAnimator.SetBool("NoZoom", true);
                break;
            case CameraDirections.ZoomInFast:
                camAnimator.SetBool("ZoomInFast", true);
                break;
            case CameraDirections.ZoomInSlow:
                camAnimator.SetBool("ZoomInSlow", true);
                break;
            case CameraDirections.ZoomOutFast:
                camAnimator.SetBool("ZoomOutFast", true);
                break;
            case CameraDirections.ZoomOutSlow:
                camAnimator.SetBool("ZoomOutSlow", true);
                break;
        }
    }
    private void HandleCamShake(CameraDirections camShake) 
    {
        camAnimator.SetBool("Steady", false);
        camAnimator.SetBool("SlightShaky", false);
        camAnimator.SetBool("VeryShaky", false);
        switch (camShake)
        {
            case CameraDirections.Steady:
                camAnimator.SetBool("Steady", true);
                break;
            case CameraDirections.SlightShaky:
                camAnimator.SetBool("SlightShaky", true);
                break;
            case CameraDirections.VeryShaky:
                camAnimator.SetBool("VeryShaky", true);
                break;
        }
    }
}
