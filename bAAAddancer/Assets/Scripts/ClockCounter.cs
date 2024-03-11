using UnityEngine;
using System.Collections;

public class ClockCounter : MonoBehaviour
{
    public delegate void BeatAction(int beatCount);
    public static event BeatAction OnBeat;
    public static event System.Action OnBeatTrigger;

    public float beatsPerMinute = 120f; // Default tempo
    public int beatsPerBar = 16; // Total beats in one bar
    public int q_BeatsPerBar = 64; // Total quarter beats in one bar
    public bool isRunning = false; // To control whether the clock is running or not

    private float beatInterval; // Duration of one beat in seconds
    private float q_BeatInterval; // Duration of one quarter beat in seconds
    private int current_Q_Beat = 0; // Current quarter beat count

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
        q_BeatInterval = beatInterval * 0.25f;
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

            Beat();
        }
    }

    // Method to handle each beat
    private void Beat()
    {
        current_Q_Beat++;
        if (current_Q_Beat > q_BeatsPerBar)
        {
            current_Q_Beat = 1; // Reset to the first beat of the bar
        }

        // Invoke event for beat count change
        OnBeat?.Invoke(current_Q_Beat);

        // Invoke event for beat trigger
        OnBeatTrigger?.Invoke();
    }
    // Method to get the current beat count
    public int GetCurrentBeat()
    {
        return current_Q_Beat;
    }
}
