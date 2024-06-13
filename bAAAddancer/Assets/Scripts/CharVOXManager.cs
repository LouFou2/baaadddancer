using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharVOXManager : MonoBehaviour
{
    [SerializeField] private AudioSource voxAudioSource;
    [SerializeField] private AudioClip casualVoxClip;
    [SerializeField] private AudioClip confusedVoxClip;
    [SerializeField] private AudioClip happyVoxClip;
    [SerializeField] private AudioClip madVoxClip;
    [SerializeField] private AudioClip pleadingVoxClip;
    [SerializeField] private AudioClip worriedVoxClip;
    public void PlayCasualVox() 
    {
        voxAudioSource.clip = casualVoxClip;
        voxAudioSource.Play();
    }
    public void PlayConfusedVox() 
    {
        voxAudioSource.clip = confusedVoxClip;
        voxAudioSource.Play();
    }
    public void PlayHappyVox() 
    {
        voxAudioSource.clip = happyVoxClip;
        voxAudioSource.Play();
    }
    public void PlayMadVox() 
    {
        voxAudioSource.clip = madVoxClip;
        voxAudioSource.Play();
    }
    public void PlayPleadingVox() 
    {
        voxAudioSource.clip = pleadingVoxClip;
        voxAudioSource.Play();
    }
    public void PlayWorriedVox() 
    {
        voxAudioSource.clip = worriedVoxClip;
        voxAudioSource.Play();
    }
}
