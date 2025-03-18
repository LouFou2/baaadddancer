using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource countIn;
    [SerializeField] private ClockCounter clockCounter;

    private bool isCountingIn = false;

    // Use one track for Count-in / Music, and switch it using play count-in or play music
    // * we need to do this because the clock counter syncs with one audio source, so it makes things easier
    [SerializeField] private AudioSource trackAudio;

    [SerializeField] private AudioClip[] musicTracks = new AudioClip[8];
    [SerializeField] private AudioClip[] countInClips = new AudioClip[8];

    private AudioClip currentMusicClip;

    private int countInIndex = -1;


    private void OnEnable()
    {
        ClockCounter.On_Beat_Trigger += On_BeatHandler; // Subscribe to the beat trigger event
    }
    private void OnDisable()
    {
        ClockCounter.On_Beat_Trigger -= On_BeatHandler; // Subscribe to the beat trigger event
    }
    void Start()
    {
        isCountingIn = true;

        clockCounter = FindObjectOfType<ClockCounter>();
        LevelKey currentLevelKey = GameManager.Instance.GetCurrentLevelKey();

        AudioClip musicTrack = music.clip;
        AudioClip countInClip = countIn.clip;

        switch (currentLevelKey) 
        {
            case LevelKey.IntroMakeDance:
                {
                    musicTrack = musicTracks[0];
                    countInClip = countInClips[0];
                    break;
                }
            case LevelKey.IntroCopyDance:
                {
                    musicTrack = musicTracks[1];
                    countInClip = countInClips[1];
                    break;
                }
            case LevelKey.Round2MakeDance:
                {
                    musicTrack = musicTracks[2];
                    countInClip = countInClips[2];
                    break;
                }
            case LevelKey.Round2CopyDance:
                {
                    musicTrack = musicTracks[3];
                    countInClip = countInClips[3];
                    break;
                }
            case LevelKey.Round3MakeDance:
                {
                    musicTrack = musicTracks[4];
                    countInClip = countInClips[4];
                    break;
                }
            case LevelKey.Round3CopyDance:
                {
                    musicTrack = musicTracks[5];
                    countInClip = countInClips[5];
                    break;
                }
            case LevelKey.Round4MakeDance:
                {
                    musicTrack = musicTracks[6];
                    countInClip = countInClips[6];
                    break;
                }
            case LevelKey.Round4CopyDance:
                {
                    musicTrack = musicTracks[7];
                    countInClip = countInClips[7];
                    break;
                }
            default:
                currentLevelKey = LevelKey.IntroMakeDance;
                break;
        }
        currentMusicClip = musicTrack;

        trackAudio.clip = countInClip;
        trackAudio.Stop();
        trackAudio.volume = 0;
        trackAudio.Play();
    }
    /*public void PlayCountIn()
    {
        trackAudio.Stop();
        trackAudio.PlayOneShot(countIn.clip);

    }
    public void PlayMusic() 
    {
        trackAudio.clip = music.clip;
        trackAudio.Stop();
        trackAudio.Play();
    }*/

    private void On_BeatHandler()
    {
        if (isCountingIn)
        {
            countInIndex++;

            if (countInIndex == 0)
            {
                trackAudio.Stop();
                trackAudio.volume = 1;
                trackAudio.Play();
            }
            //count to 4 and then switch to music

            if (countInIndex == 4) // on count 5 the music starts
            {
                isCountingIn = false;

                // Play the music only once when the count-in finishes
                trackAudio.Stop();
                trackAudio.clip = currentMusicClip;
                trackAudio.Play();
            }
        }
        
    }
}
