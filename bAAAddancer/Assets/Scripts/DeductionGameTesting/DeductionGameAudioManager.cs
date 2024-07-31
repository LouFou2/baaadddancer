using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeductionGameAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audio;

    public void PlayAudio() 
    {
        audio.Play();
    }
}
