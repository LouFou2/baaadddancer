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
        ClockCounter.OnBeatTrigger += OnBeatTriggered;
    }

    void OnDestroy()
    {
        // Unsubscribe from the OnBeatTrigger event to avoid memory leaks
        ClockCounter.OnBeatTrigger -= OnBeatTriggered;
    }

    // Method called when a beat is triggered
    void OnBeatTriggered()
    {
        int currentBeat = clock.GetCurrentBeat(); // Get the current beat count from the ClockCounter script

        // Check if the current beat should trigger the audio clip based on the beat pattern
        if (beatPattern[currentBeat - 1])
        {
            // Play the audio clip
            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.PlayOneShot(audioSource.clip);
            }
        }
    }
}
