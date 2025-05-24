using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGameAudioManager : MonoBehaviour //the debug audio is supervised by the DebugUI_Manager
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AlignerController2 alignController;
    private ClockCounter clockCounter;

    private float tempo;
    private float beatInterval;

    [SerializeField] [Range(0,1)] private float volume;
    [SerializeField] [Range(0.25f, 2)] private float pitch;
    [SerializeField] [Range(400, 20000)] private float cutOff;
    [SerializeField] [Range(0, 1)] private float delayWet;
    [SerializeField] [Range(10, 60)] private float delayAmount;

    private float initVolume;
    private float initPitch;
    private float initCutOff;
    private float initDelayWet;
    private float initDelayAmount;

    public bool debugUIRunning = false; //this gets triggered from the DebugUI_Manager
    public bool alignerGameRunning = false; //also this

    private void OnEnable()
    {
        AudioManager.On_TrackStarted += TrackStartedHandler; // we use this so we only set all parameters AFTER track play starts (else volume is still 0)
    }
    private void OnDisable()
    {
        AudioManager.On_TrackStarted -= TrackStartedHandler;
    }

    void Start()
    {
        clockCounter = FindObjectOfType<ClockCounter>();
        tempo = clockCounter.GetTempo();
    }
    void TrackStartedHandler()
    {
        initVolume = audioSource.volume;
        initPitch = audioSource.pitch;
        initCutOff = audioSource.GetComponent<AudioLowPassFilter>().cutoffFrequency;
        initDelayAmount = audioSource.GetComponent<AudioEchoFilter>().wetMix;
        initDelayWet = audioSource.GetComponent<AudioEchoFilter>().delay;
        
        volume = initVolume;
        pitch = initPitch;
        cutOff = initCutOff;
        delayAmount = initDelayAmount;
        delayWet = initDelayWet;
    }
    // these methods are called from DialogueManager_002 === 
    public void StartDebugUIAudio() 
    {
        debugUIRunning = true;
    }
    public void EndDebugUIAudio()
    {
        debugUIRunning = false;
    }
    public void StartAlignerAudio()
    {
        alignerGameRunning = true;
    }
    public void EndAlignerAudio()
    {
        Debug.Log("reset audio");
        alignerGameRunning = false;

        audioSource.volume = initVolume;
        audioSource.pitch = initPitch;
        clockCounter.SetTempo(134);
        audioSource.GetComponent<AudioLowPassFilter>().cutoffFrequency = initCutOff;
        audioSource.GetComponent<AudioEchoFilter>().delay = initDelayAmount;
        audioSource.GetComponent<AudioEchoFilter>().wetMix = initDelayWet;
    }
    // ===

    void Update()
    {
        if (!debugUIRunning)
        {
            return;
        }

        //Logic to manipulate Audio Effects

        beatInterval = clockCounter.GetBeatInterval();

        float alignAmountX = alignController.GetAlignedX();
        float alignAmountY = alignController.GetAlignedY();
        float thumbMagX = alignController.GetThumbMagnitudeX();
        float thumbMagY = alignController.GetThumbMagnitudeY();
        float barX = alignController.GetBarPosX();
        float barY = alignController.GetBarPosY();

        //So we just have to decide how to use the 4 value ranges:

        cutOff = Mathf.Lerp(240, 22000, thumbMagY);
        delayWet = Mathf.Lerp(1, 0, alignAmountY);
        delayAmount = Mathf.Lerp(beatInterval * 64, beatInterval , alignAmountY);
        pitch = Mathf.Lerp(0.1f, 1.5f, thumbMagX);

        volume = Mathf.Lerp(0.2f, 1f, barX);
        cutOff *= Mathf.Lerp(0.05f, 1f, barY); // Note: we multiply the above cutOff value again here, so its an extra factor

        // set all the values
        if (alignerGameRunning)
        {
            audioSource.volume = volume;

            audioSource.pitch = pitch;
            clockCounter.SetTempo(tempo * pitch);

            audioSource.GetComponent<AudioLowPassFilter>().cutoffFrequency = cutOff;
            audioSource.GetComponent<AudioEchoFilter>().delay = delayAmount;
            audioSource.GetComponent<AudioEchoFilter>().wetMix = delayWet;
        }
        else EndAlignerAudio(); // just repeating because the update is weird

        
    }

    
}
