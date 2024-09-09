using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeductionGameAudioManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    public void PlayAudio() 
    {
        audioSource.Play();
    }
}
