using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonFaceScreen : MonoBehaviour
{
    [SerializeField] private ClockCounter clockCounter; // assign in inspector
    [SerializeField] private ScreenManipulator screenManipulator; // assign in inspector
    [SerializeField] private Material matDemonFaceScreen; // assign in inspector
    [SerializeField] private Material screenRenderTextureMat; // assign in inspector

    [SerializeField] private float offsetY = -0.5f;
    [SerializeField] private float scaleMovementY = 0.5f;

    private Vector2 mouthPos;

    private float time = 0;
    private float beatInterval;

    private void OnEnable()
    {
        ClockCounter.On_Beat_Trigger += OnBeatHandler; // Subscribe to the beat trigger event
    }
    private void OnDisable()
    {
        ClockCounter.On_Beat_Trigger -= OnBeatHandler; // Unsubscribe to the beat trigger event
    }

    void Update()
    {
        time += Time.deltaTime;

        Vector2 thumbL = screenManipulator.GetThumbVectorL();
        Vector2 thumbR = screenManipulator.GetThumbVectorR();
        float thumbMagL = screenManipulator.GetThumbMagL();
        float thumbMagR = screenManipulator.GetThumbMagR();

        //placing the center of the mouth between the two thumb inputs:
        float thumbCenterX = (thumbR.x - thumbL.x) * 0.5f;
        float thumbCenterY = (thumbR.y - thumbL.y) * 0.5f;

        float positionX = thumbL.x + thumbCenterX;
        float positionY = thumbL.y + offsetY + (thumbCenterY * scaleMovementY);

        mouthPos = new Vector2(positionX, positionY);

        matDemonFaceScreen.SetVector("_MouthCenter", mouthPos);
        screenRenderTextureMat.SetVector("_MouthCenter", mouthPos);

        matDemonFaceScreen.SetVector("_ThumbL", thumbL);
        matDemonFaceScreen.SetVector("_ThumbR", thumbR);
        matDemonFaceScreen.SetFloat("_ThumbMagL", thumbMagL);
        matDemonFaceScreen.SetFloat("_ThumbMagR", thumbMagR);

        //mouth open close - every beat interval open AND close, in a sine movement:
        //float invLerpBeat = Mathf.InverseLerp(0, beatInterval, time);
        float sinTime = Mathf.Sin((time / beatInterval) * Mathf.PI * 2f);
        sinTime = (sinTime + 1f) * 0.5f; // remap to 0–1
        float mouthOpenAmount = Mathf.Lerp(0.1f, 2.0f, sinTime);

        matDemonFaceScreen.SetFloat("_MouthOpener", mouthOpenAmount);
    }
    public Vector2 GetMouthCenter()
    {
        return mouthPos;
    }
    void OnBeatHandler()
    {
        beatInterval = clockCounter.GetBeatInterval();
        time = 0;
    }
}
