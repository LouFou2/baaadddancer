using UnityEngine;
using System.Collections;

public class ClockCounter : MonoBehaviour
{
    public static event System.Action On_Q_Beat_Trigger;
    public static event System.Action On_Beat_Trigger;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip; // *** CAN REMOVE THIS, IT IS JUST FOR DEBUGGING
    public float beatsPerMinute = 134f; // Default tempo
    [SerializeField] private float steps = 1f;

    public int q_BeatsPerBar = 64; // Total quarter beats in one bar
    public bool isRunning = false; // To control whether the clock is running or not

    private float q_BeatInterval; // Duration of one quarter beat in seconds
    private float beatInterval; // Duration of one beat in seconds
    private int current_Q_Beat = 0; // Current quarter beat count
    private int currentBeat = 0;
    private int lastInterval;

    void Start()
    {
        SetTempo(beatsPerMinute);
    }

    // Method to set the tempo
    public void SetTempo(float bpm)
    {
        beatsPerMinute = bpm;
        beatInterval = 60f / (bpm * steps);
        q_BeatInterval = beatInterval * 0.25f; // calculate for quarter beats
    }
    private void Update()
    {
        audioClip = audioSource.clip;
        float sampledTime = (audioSource.timeSamples / (audioSource.clip.frequency * q_BeatInterval));
        CheckForNewInterval(sampledTime);
    }
/*
    // Coroutine for beat timing
    private IEnumerator BeatCoroutine()
    {
        while (isRunning)
        {
            yield return new WaitForSeconds(q_BeatInterval);

            Q_Beat(); // Quarter beats are used for music sequencer
        }
    }
*/
    
    public void CheckForNewInterval(float interval)
    {
        if (Mathf.FloorToInt(interval) != lastInterval)
        {
            lastInterval = Mathf.FloorToInt(interval);
            // Invoke event for beat trigger
            On_Q_Beat_Trigger?.Invoke();
            IncrementCounts();
        }
    }
    public void IncrementCounts() //this method should be triggered every quarter beat
    {
        current_Q_Beat++;

        if (current_Q_Beat > q_BeatsPerBar - 1)
        {
            current_Q_Beat = 0; // Reset to the first beat of the bar
        }

        if (current_Q_Beat == 0 || current_Q_Beat % 4 == 0)
        {
            On_Beat_Trigger.Invoke();
            currentBeat = (int)(current_Q_Beat * 0.25f);
        }
    }

/*
    // Old Method to handle each beat
    private void Q_Beat() // Quarter beats are used for music sequencer
    {
        current_Q_Beat++;
        if (current_Q_Beat > q_BeatsPerBar - 1)
        {
            current_Q_Beat = 0; // Reset to the first beat of the bar
        }

        // Invoke event for beat trigger
        On_Interval_Trigger?.Invoke();

        // Invoke event for beat trigger (every 4 quarter beats)
        if (current_Q_Beat == 0 || current_Q_Beat % 4 == 0)
        {
            currentBeat = current_Q_Beat == 0 ? 0 : (int)(current_Q_Beat * 0.25f);
        }
    }
*/

    // Method to get the current beat count
    public int GetCurrent_Q_Beat()
    {
        return current_Q_Beat;
    }
    public int GetCurrentBeat() 
    {
        return currentBeat;
    }
    public float Get_Q_BeatInterval()
    {
        if (audioSource.clip == null) return q_BeatInterval;
        float samplesPerBeat = audioSource.clip.frequency * (60f / (beatsPerMinute * steps));
        return samplesPerBeat * 0.25f / audioSource.clip.frequency;
    }
    public float GetBeatInterval() 
    {
        if (audioSource.clip == null) return beatInterval;

        float samplesPerBeat = audioSource.clip.frequency * (60f / (beatsPerMinute * steps));
        return samplesPerBeat / audioSource.clip.frequency;
    }
}


