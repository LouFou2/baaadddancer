using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCounter : MonoBehaviour
{
    [SerializeField] private float beatsPerBar;
    public float beat;
    [SerializeField] private float barsPerLoop;
    public float bar;

    public float beatTime;
    public float barTime;
    public float clockSpeed = 0.1f;

    public float counterTime = 0;

    void Update()
    {
        counterTime = Time.deltaTime * clockSpeed;
        beatTime += counterTime;
        barTime += counterTime * 0.25f;

        beat = Mathf.Floor( beatTime );
        if (beat == beatsPerBar)
        {
            beatTime = 0;
        }

        bar = Mathf.Floor( barTime );
        if (bar == barsPerLoop)
        {
            barTime = 0;
        }

    }
}
