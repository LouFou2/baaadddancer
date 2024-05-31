using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutscenesAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource oneShot;
    [SerializeField] private AudioSource vox;
    [SerializeField] private AudioClip[] musicTracks = new AudioClip[8];

    [SerializeField] private Queue<AudioClip> oneshotClips =  new Queue<AudioClip>();
    [SerializeField] private Queue<AudioClip> newMusicClips = new Queue<AudioClip>();

    [SerializeField] private DialogueSwitcher dialogueSwitcher;

    void Start()
    {
        LevelKey currentLevelKey = GameManager.Instance.GetCurrentLevelKey();
        dialogueSwitcher = FindObjectOfType<DialogueSwitcher>();

        //AudioClip musicTrack = null;

        QueueOneShots();
        QueueMusicClips();
/*
        switch (currentLevelKey)  // can cueue the tracks that need to play at start of scene
        {
            case LevelKey.IntroDialogue:
                {
                    musicTrack = musicTracks[0];
                    break;
                }
            case LevelKey.IntroMeeting:
                {
                    musicTrack = musicTracks[1];
                    break;
                }
            case LevelKey.Round2Dialogue:
                {
                    musicTrack = musicTracks[2];
                    break;
                }
            case LevelKey.Round2Meeting:
                {
                    musicTrack = musicTracks[3];
                    break;
                }
            case LevelKey.Round3Dialogue:
                {
                    musicTrack = musicTracks[4];
                    break;
                }
            case LevelKey.Round3Meeting:
                {
                    musicTrack = musicTracks[5];
                    break;
                }
            case LevelKey.Round4Dialogue:
                {
                    musicTrack = musicTracks[6];
                    break;
                }
            case LevelKey.Round4Meeting:
                {
                    musicTrack = musicTracks[7];
                    break;
                }
            default:
                currentLevelKey = LevelKey.IntroDialogue;
                break;
        }
        music.clip = musicTrack;*/
        PlayNewMusicTrack();
    }
    public void QueueMusicClips()
    {
        DialogueData currentDialogueData = dialogueSwitcher.GetCurrentDialogue();
        newMusicClips.Clear();

        foreach (DialogueData.DialogueUnit unit in currentDialogueData.dialogueUnits)
        {
            newMusicClips.Enqueue(unit.oneshotClipToPlay);
        }
        PlayMusic(); //will only play if there is an audioClip to play
    }
    public void PlayMusic()
    {
        if (newMusicClips.Count > 0) 
        {
            AudioClip queuedMusicClip = newMusicClips.Dequeue();
            if (queuedMusicClip != null)
            {
                music.clip = queuedMusicClip;
                music.Stop();
                music.Play();
            }
        }
    }
    public void PlayNewMusicTrack() // I call this with TriggerNextDialogue Event in DialogueManager (UnityEvent: set in Inspector)
    {
        AudioClip musicToPlay = null; 
        musicToPlay = dialogueSwitcher.GetCurrentDialogue().musicTrackToPlay;

        if (musicToPlay != null) // It will only play a new track if a track is added to the new Dialogue Data
        {
            music.Stop();
            music.clip = musicToPlay;
            music.Play();
        }
        else return;
    }
    
    public void QueueOneShots() // Also UnityEvent on Dialogue Manager (in inspector)
    {
        DialogueData currentDialogueData = dialogueSwitcher.GetCurrentDialogue();
        oneshotClips.Clear();

        foreach (DialogueData.DialogueUnit unit in currentDialogueData.dialogueUnits) 
        {
            oneshotClips.Enqueue(unit.oneshotClipToPlay);
        }
        PlayOneShot(); //will only play if there is an audioClip to play
    }
    public void PlayOneShot() //will only play if there is an audioClip to play *
    {
        if (oneshotClips.Count > 0)
        {
            AudioClip oneShotClip = oneshotClips.Dequeue();

            if (oneShotClip != null) // *
            {
                oneShot.clip = oneShotClip;
                oneShot.Play();
            }
        }
    }

    public void playVox(AudioClip voxClipToPlay)
    {
        vox.clip = voxClipToPlay;
        vox.Stop();
        vox.Play();
    }
}
