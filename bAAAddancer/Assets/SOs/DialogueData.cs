using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public class DialogueUnit {
        public SpeakingCharacter speakingCharacter;
        [TextArea(3, 10)]
        public string sentence;
        public CameraToSwitch Camera;
        public LookPosition charsLookPosition;
        public LookPosition speakerLookPosition;
        public AudioClip oneshotClipToPlay;
        public AudioClip newMusicTrackToPlay;
        public AudioClip voxClipToPlay;
    }

    public AudioClip musicTrackToPlay; // the main track of the dialogue unit (different from newMusicTrackToPlay that can be timed with dialogue beats)

    public DialogueUnit[] dialogueUnits;

    public string responseNo;
    public string responseYes;
    public LookPosition playerLookPosition;
    public AudioClip responseOneshotClip;

    public enum EventsToCall { triggerNextDialogue, switchScene, customEvent }
    public EventsToCall NoEventToCall;
    public EventsToCall YesEventToCall;

    
    public enum SpeakingCharacter 
    {
        playerSpeaking, demonSpeaking, lastBuggedSpeaking, npc02Speaking, npc03Speaking, npc04Speaking
    }

    public enum CameraToSwitch {
        longCam,
        playerCamM, playerCamC,
        ravedemonCamM, ravedemonCamC,
        npc02CamM, npc02CamC,
        npc03CamM, npc03CamC,
        npc04CamM, npc04CamC,
        lastBuggedCamM, lastBuggedCamC
    }
    public enum LookPosition
    {
        LookatCamera, LookatPlayer, LookatDemon, LookatLastBuggedNPC, LookatNPC2, LookatNPC3, LookatNPC4
    }
    
}