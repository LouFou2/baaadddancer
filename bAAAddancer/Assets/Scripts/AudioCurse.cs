using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCurse : MonoBehaviour
{
    private ClockCounter clock;
    private AudioDistortionFilter distortion;
    private AudioHighPassFilter highPass;

    void Start()
    {
        clock = FindObjectOfType<ClockCounter>();
        distortion = GetComponent<AudioDistortionFilter>();
        highPass = GetComponent<AudioHighPassFilter>();

        distortion.distortionLevel = 0f;
        highPass.cutoffFrequency = 10;
    }

    public void CurseAudio() 
    {
        StartCoroutine(CurseAudioRoutine());
    }
    private IEnumerator CurseAudioRoutine() 
    {
        float qBeatDuration = clock.Get_Q_BeatInterval();

        distortion.distortionLevel = 1f;
        highPass.cutoffFrequency = 1800;
        yield return new WaitForSeconds(qBeatDuration * 2);

        distortion.distortionLevel = 0f;
        highPass.cutoffFrequency = 10;
        yield return new WaitForSeconds(qBeatDuration * 2);

        distortion.distortionLevel = 1f;
        highPass.cutoffFrequency = 1800;
        yield return new WaitForSeconds(qBeatDuration);

        distortion.distortionLevel = 0f;
        highPass.cutoffFrequency = 10;
        yield return new WaitForSeconds(qBeatDuration);

        distortion.distortionLevel = 1f;
        highPass.cutoffFrequency = 1800;
        yield return new WaitForSeconds(qBeatDuration);


        distortion.distortionLevel = 0f; //return this to normal
        highPass.cutoffFrequency = 10;
    }
}
