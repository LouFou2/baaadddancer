using UnityEngine;
using System.Collections;

public class ClockCounter : MonoBehaviour
{
    public delegate void BeatAction(int beatCount); //*** CAN THESE TWO LINES 
    public static event BeatAction On_Q_Beat;       // BE REMOVED?
    public static event System.Action On_Q_BeatTrigger;
    public static event System.Action OnBeatTrigger;

    public float beatsPerMinute = 120f; // Default tempo
    public int beatsPerBar = 16; // Total beats in one bar
    public int q_BeatsPerBar = 64; // Total quarter beats in one bar
    public bool isRunning = false; // To control whether the clock is running or not

    private float beatInterval; // Duration of one beat in seconds
    private float q_BeatInterval; // Duration of one quarter beat in seconds
    private int current_Q_Beat = 0; // Current quarter beat count
    private int currentBeat = 0;

    void Start()
    {
        SetTempo(beatsPerMinute);
        StartClock();
    }

    void Update()
    {
        SetTempo(beatsPerMinute);
    }

    // Method to set the tempo
    public void SetTempo(float bpm)
    {
        beatsPerMinute = bpm;
        beatInterval = 60f / beatsPerMinute;
        q_BeatInterval = beatInterval * 0.25f; // calculate for quarter beats
    }

    // Method to start the clock
    public void StartClock()
    {
        isRunning = true;
        StartCoroutine(BeatCoroutine());
    }

    // Method to stop the clock
    public void StopClock()
    {
        isRunning = false;
        StopAllCoroutines();
    }

    // Coroutine for beat timing
    private IEnumerator BeatCoroutine()
    {
        while (isRunning)
        {
            yield return new WaitForSeconds(q_BeatInterval);

            Q_Beat(); // Quarter beats are used for music sequencer
        }
    }

    // Method to handle each beat
    private void Q_Beat() // Quarter beats are used for music sequencer
    {
        current_Q_Beat++;
        if (current_Q_Beat > q_BeatsPerBar - 1)
        {
            current_Q_Beat = 0; // Reset to the first beat of the bar
        }

        // Invoke event for beat count change
        On_Q_Beat?.Invoke(current_Q_Beat);

        // Invoke event for beat trigger
        On_Q_BeatTrigger?.Invoke();

        // Invoke event for beat trigger (every 4 quarter beats)
        if (current_Q_Beat == 0 || current_Q_Beat % 4 == 0)
        {
            OnBeatTrigger?.Invoke();
            currentBeat = current_Q_Beat == 0 ? 0 : (int)(current_Q_Beat * 0.25f);
        }
    }
    // Method to get the current beat count
    public int GetCurrent_Q_Beat()
    {
        return current_Q_Beat;
    }
    public int GetCurrentBeat() 
    {
        return currentBeat;
    }
}
