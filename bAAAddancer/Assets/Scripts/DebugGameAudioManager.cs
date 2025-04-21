using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGameAudioManager : MonoBehaviour
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

    public bool alignerGameRunning = false; //this gets triggered from the DebugUI_Manager



    void Start()
    {
        clockCounter = FindObjectOfType<ClockCounter>();
        tempo = clockCounter.GetTempo();

        volume = audioSource.volume;
        pitch = audioSource.pitch;
        cutOff = audioSource.GetComponent<AudioLowPassFilter>().cutoffFrequency;
        delayAmount = audioSource.GetComponent<AudioEchoFilter>().delay;
        delayWet = audioSource.GetComponent<AudioEchoFilter>().wetMix;
    }

    void Update()
    {
        //*** TODO: WE NEED LOGIC TO START "ALIGNER GAME RUNNING"



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
