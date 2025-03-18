using UnityEngine;

public class AudioSequencer : MonoBehaviour
{
    //public AudioClip audioClip; // Audio clip to play
    public bool[] beatPattern = new bool[64]; // Array to store beat triggers

    private AudioSource audioSource;
    private ClockCounter clock;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        clock = FindObjectOfType<ClockCounter>(); // Find the ClockCounter script in the scene

        // Subscribe to the OnBeatTrigger event
        ClockCounter.On_Q_Beat_Trigger += On_Q_BeatTriggered;
    }

    void OnDestroy()
    {
        // Unsubscribe from the OnBeatTrigger event to avoid memory leaks
        ClockCounter.On_Q_Beat_Trigger -= On_Q_BeatTriggered;
    }

    // Method called when a beat is triggered
    void On_Q_BeatTriggered()
    {
        int currentBeat = clock.GetCurrent_Q_Beat(); // Get the current beat count from the ClockCounter script

        // Check if the current beat should trigger the audio clip based on the beat pattern
        if (beatPattern[currentBeat])
        {
            // Play the audio clip
            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.PlayOneShot(audioSource.clip);
            }
        }
    }
}
