using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public class DialogueUnit {
        [TextArea(3, 10)]
        public string sentence;
        public SpeakingCharacter speakingCharacter;
        public CameraToSwitch Camera;
        public LookPosition speakerLookPosition;
        public LookPosition charsLookPosition;
        public AudioClip oneshotClipToPlay;
        public AudioClip newMusicTrackToPlay;
        public AudioClip voxClipToPlay;
        public VoxEmote voxEmote;
    }

    public AudioClip musicTrackToPlay; // the main track of the dialogue unit (different from newMusicTrackToPlay that can be timed with dialogue beats)

    public DialogueUnit[] dialogueUnits;

    public string responseNo;
    public string responseYes;
    public LookPosition playerLookPosition;
    public AudioClip responseOneshotClip;
    public VoxEmote responseEmote;

    public enum ResponseEvents { triggerNextDialogue, switchScene, customEvent }

    public ResponseEvents NoEventToCall;
    public ResponseEvents YesEventToCall;


    public enum SpeakingCharacter
    {
        playerSpeaking, demonSpeaking, lastBuggedSpeaking, npc01Speaking, npc02Speaking, npc03Speaking, npc04Speaking, npc05Speaking
    }

    public enum VoxEmote
    {
        casual, confused, happy, mad, pleading, worried
    }

    public enum AnimEmote
    {
        docile, afraid, exuberant, hostile,
    }

    public enum CameraToSwitch
    {
        longCam,
        playerCamM, playerCamC,
        ravedemonCamM, ravedemonCamC,
        lastBuggedCamM, lastBuggedCamC,
        npc01CamM, npc01CamC,
        npc02CamM, npc02CamC,
        npc03CamM, npc03CamC,
        npc04CamM, npc04CamC,
        npc05CamM, npc05CamC,

    }
    public enum LookPosition
    {
        LookatCamera,
        LookatPlayer, LookatDemon, LookatLastBuggedNPC,
        LookatNPC1, LookatNPC2, LookatNPC3, LookatNPC4, LookatNPC5,
        LookAround,
    }

}