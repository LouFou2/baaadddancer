using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutscenesAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource oneShot;
    [SerializeField] private AudioSource vox;

    [SerializeField] private Queue<AudioClip> oneshotClips =  new Queue<AudioClip>();
    [SerializeField] private Queue<AudioClip> newMusicClips = new Queue<AudioClip>();

    [SerializeField] private DialogueSwitcher dialogueSwitcher;

    private HashSet<AudioClip> playedResponseClips = new HashSet<AudioClip>();

    void Start()
    {
        LevelKey currentLevelKey = GameManager.Instance.GetCurrentLevelKey();
        dialogueSwitcher = FindObjectOfType<DialogueSwitcher>();

        //AudioClip musicTrack = null;

        QueueOneShots();
        QueueMusicClips();
        PlayNewMusicTrack();
    }

    // ---------------------
    // PlayMusicTrack method handles the "musicTrackToPlay" from DialogueData:
    public void PlayNewMusicTrack() // I call this with TriggerNextDialogue Event in DialogueManager (UnityEvent: set in Inspector)
    {
        AudioClip musicToPlay = dialogueSwitcher.GetCurrentDialogue().musicTrackToPlay;

        if (musicToPlay != null) // It will only play a new track if a track is added to the new Dialogue Data
        {
            music.Stop();
            music.clip = musicToPlay;
            music.Play();
        }
        else return;
    }
    public void PlayResponseOneShot() 
    {
        AudioClip currentClip = dialogueSwitcher.GetCurrentDialogue().responseOneshotClip;

        // Check if the current clip is not null and hasn't been played yet
        if (currentClip != null && !playedResponseClips.Contains(currentClip))
        {
            oneShot.clip = currentClip;
            oneShot.Play();
            playedResponseClips.Add(currentClip);
        }
    }
    //---------------------------
    // QUEUES:
    public void QueueOneShots() // Also UnityEvent on Dialogue Manager (in inspector)
    {
        DialogueData currentDialogueData = dialogueSwitcher.GetCurrentDialogue();
        oneshotClips.Clear();

        foreach (DialogueData.DialogueUnit unit in currentDialogueData.dialogueUnits)
        {
            oneshotClips.Enqueue(unit.oneshotClipToPlay); // *** NB it also enqueues the null audioclips
        }
        PlayOneShot(); //will only play if there is an audioClip to play //*** it skips the null clips, but still "dequeues" it on the call
    }
    public void QueueMusicClips()
    {
        DialogueData currentDialogueData = dialogueSwitcher.GetCurrentDialogue();
        newMusicClips.Clear();

        foreach (DialogueData.DialogueUnit unit in currentDialogueData.dialogueUnits)
        {
            newMusicClips.Enqueue(unit.newMusicTrackToPlay); // *** NB it also enqueues the null audioclips
        }
        PlayMusic(); //will only play if there is an audioClip to play //*** it skips the null clips, but still "dequeues" it on the call
    }
    //--------------------------------------
    // PLAYS:
    public void PlayMusic() // this method handles the queue of "newMusicTrackToPlay" clips from Dialogue units
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

    public void PlayVox(AudioClip voxClipToPlay)
    {
        vox.clip = voxClipToPlay;
        vox.Stop();
        vox.Play();
    }
}
