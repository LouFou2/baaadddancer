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

    private Queue<DialogueData.DialogueUnit> dialogueUnitQueue = new Queue<DialogueData.DialogueUnit>();

    [SerializeField] private DialogueSwitcher dialogueSwitcher;
    [SerializeField] private CharacterManager characterManager;

    [SerializeField] private CharVOXManager[] charVOXManagers; // add in inspector, in same order as Character Manager char data SO's

    // Indexes to use, same as CameraManager, according to dialogue data + character data
    private CharacterData[] characterDataSOs;
    private List<int> availableIndexes = new List<int>();
    private int playerIndex;
    private int bugIndex;
    private int lastBuggedCharacterIndex;
    private int npc1_Index, npc2_Index, npc3_Index; //remaining npcs

    private int speakingCharacter = -1;

    private HashSet<AudioClip> playedResponseClips = new HashSet<AudioClip>();
    private bool playerResponseVoxPlayed = false;

    void Start()
    {
        LevelKey currentLevelKey = GameManager.Instance.GetCurrentLevelKey();
        dialogueSwitcher = FindObjectOfType<DialogueSwitcher>();

        characterDataSOs = new CharacterData[6];

        availableIndexes.Clear();
        // Populate available indexes
        for (int i = 0; i < characterManager.characterDataSOs.Length; i++)
        {
            availableIndexes.Add(i);
        }
        for (int i = 0; i < characterManager.characterDataSOs.Length; i++)
        {
            characterDataSOs[i] = characterManager.characterDataSOs[i];
            if (characterDataSOs[i].characterRoleSelect == CharacterData.CharacterRole.Player)
            {
                playerIndex = i;
                availableIndexes.Remove(i); // Remove player index
            }
            if (characterDataSOs[i].characterRoleSelect == CharacterData.CharacterRole.Demon)
            {
                bugIndex = i;
                availableIndexes.Remove(i); // Remove bug index
            }
            if (characterDataSOs[i].lastBuggedCharacter == true)
            {
                lastBuggedCharacterIndex = i;
                availableIndexes.Remove(i); // Remove last bugged character index
            }
        }
        // Assign remaining indexes to NPC characters
        npc1_Index = availableIndexes[0];
        npc2_Index = availableIndexes[1];
        npc3_Index = availableIndexes[2];

        DialogueData currentDialogue = dialogueSwitcher.GetCurrentDialogue();
        QueueDialogueUnits(currentDialogue);

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
    public void PlayResponseVOX()
    {
        if (playerResponseVoxPlayed) 
        {
            return;
        }
        
        DialogueData.VoxEmote responseEmote = dialogueSwitcher.GetCurrentDialogue().responseEmote;

        switch (responseEmote)
        {
            case DialogueData.VoxEmote.casual:
                charVOXManagers[playerIndex].PlayCasualVox();
                break;
            case DialogueData.VoxEmote.confused:
                charVOXManagers[playerIndex].PlayConfusedVox();
                break;
            case DialogueData.VoxEmote.happy:
                charVOXManagers[playerIndex].PlayHappyVox();
                break;
            case DialogueData.VoxEmote.mad:
                charVOXManagers[playerIndex].PlayMadVox();
                break;
            case DialogueData.VoxEmote.pleading:
                charVOXManagers[playerIndex].PlayPleadingVox();
                break;
            case DialogueData.VoxEmote.worried:
                charVOXManagers[playerIndex].PlayWorriedVox();
                break;

            default:
                charVOXManagers[playerIndex].PlayCasualVox();
                break;
        }
        playerResponseVoxPlayed = true;
    }
    //-------------------
    // SWITCHES:
    // speaker
    void SwitchCurrentSpeaker(DialogueData.SpeakingCharacter currentSpeaker)
    {
        switch (currentSpeaker)
        {
            case DialogueData.SpeakingCharacter.playerSpeaking:
                speakingCharacter = playerIndex;
                break;
            case DialogueData.SpeakingCharacter.demonSpeaking:
                speakingCharacter = bugIndex;
                break;
            case DialogueData.SpeakingCharacter.lastBuggedSpeaking:
                speakingCharacter = lastBuggedCharacterIndex;
                break;
            case DialogueData.SpeakingCharacter.npc01Speaking:
                speakingCharacter = npc1_Index;
                break;
            case DialogueData.SpeakingCharacter.npc02Speaking:
                speakingCharacter = npc2_Index;
                break;
            case DialogueData.SpeakingCharacter.npc03Speaking:
                speakingCharacter = npc3_Index;
                break;
            default:
                speakingCharacter = playerIndex;
                break;
        }
    }
    // vox emotes
    void SwitchVoxEmote(DialogueData.VoxEmote currentVoxEmote) 
    {
        switch (currentVoxEmote) 
        {
            case DialogueData.VoxEmote.casual:
                charVOXManagers[speakingCharacter].PlayCasualVox();
                break;
            case DialogueData.VoxEmote.confused:
                charVOXManagers[speakingCharacter].PlayConfusedVox();
                break;
            case DialogueData.VoxEmote.happy:
                charVOXManagers[speakingCharacter].PlayHappyVox();
                break;
            case DialogueData.VoxEmote.mad:
                charVOXManagers[speakingCharacter].PlayMadVox();
                break;
            case DialogueData.VoxEmote.pleading:
                charVOXManagers[speakingCharacter].PlayPleadingVox();
                break;
            case DialogueData.VoxEmote.worried:
                charVOXManagers[speakingCharacter].PlayWorriedVox();
                break;

            default:
                charVOXManagers[speakingCharacter].PlayCasualVox();
                break;
        }

    }
    //---------------------------
    // QUEUES:
    public void QueueDialogueUnits(DialogueData newDialogue) // this gets called from DialogueSwitcher.TriggerNextDialogue()
    {
        DialogueData currentDialogueData = dialogueSwitcher.GetCurrentDialogue();
        dialogueUnitQueue.Clear();

        playerResponseVoxPlayed = false;

        foreach (DialogueData.DialogueUnit unit in newDialogue.dialogueUnits)
        {
            dialogueUnitQueue.Enqueue(unit);
        }
        ChangeSpeakerAndPlayVOX();
    }
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
    //--------------------------------------
    // set up speaker:
    public void ChangeSpeakerAndPlayVOX()  // *** HAS to be called from DialogueManager, same as "lookManager.SelectNewLookPosition() line 64"
    {
        if (dialogueUnitQueue.Count > 0)
        {
            DialogueData.DialogueUnit unit = dialogueUnitQueue.Dequeue();
            SwitchCurrentSpeaker(unit.speakingCharacter);
            SwitchVoxEmote(unit.voxEmote);
        }
    }
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
    
}
