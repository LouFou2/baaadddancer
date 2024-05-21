using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource countIn;
    [SerializeField] private AudioClip[] musicTracks = new AudioClip[8];
    [SerializeField] private AudioClip[] countInClips = new AudioClip[8];

    void Start()
    {
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
        music.clip = musicTrack;
        countIn.clip = countInClip;
    }

    public void playMusic() 
    {
        music.Stop();
        music.Play();
    }
    public void playCountIn()
    {
        countIn.Stop();
        countIn.Play();
    }
}
