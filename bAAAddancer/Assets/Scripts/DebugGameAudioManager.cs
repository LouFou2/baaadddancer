using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGameAudioManager : MonoBehaviour //the debug audio is supervised by the DebugUI_Manager
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AlignerController2 alignController;
    private ClockCounter clockCounter;

    private float tempo;

    [SerializeField] [Range(0,1)] private float volume;
    [SerializeField] [Range(0.25f, 2)] private float pitch;
    [SerializeField] [Range(400, 20000)] private float cutOff;
    [SerializeField] [Range(0, 1)] private float delayWet;
    [SerializeField] [Range(10, 60)] private float delayAmount;

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
        volume = audioSource.volume;
        pitch = audioSource.pitch;
        cutOff = audioSource.GetComponent<AudioLowPassFilter>().cutoffFrequency;
        delayAmount = audioSource.GetComponent<AudioEchoFilter>().delay;
        delayWet = audioSource.GetComponent<AudioEchoFilter>().wetMix;
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
        alignerGameRunning = false;
    }
    // ===

    void Update()
    {
        if (!debugUIRunning)
        {
            return;
        }

        //Logic to manipulate Audio Effects
        float alignAmount = 1 - alignController.GetFinalAlignedAmount();
        cutOff = Mathf.Lerp(240, 22000, alignAmount);
        delayWet = Mathf.Lerp(1, 0, alignAmount);
        delayAmount = Mathf.Lerp(100, 10, alignAmount);
        pitch = Mathf.Lerp(1.5f, 1, alignAmount);

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
        
    }

    
}
