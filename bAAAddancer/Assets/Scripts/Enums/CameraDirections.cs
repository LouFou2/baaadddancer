[System.Serializable]
public enum CameraDirections 
{
    //camera
    SpeakingCharCam, SpokenToCharCam, LongCam,
    //distance
    CamMed, CamClose, CamXClose,
    //angle
    CamAngleHigh, CamAngleLevel, CamAngleLow,
    //zoom
    NoZoom, ZoomInFast, ZoomInSlow, ZoomOutFast, ZoomOutSlow,
    //shake
    Steady, SlightShaky, VeryShaky,
}
