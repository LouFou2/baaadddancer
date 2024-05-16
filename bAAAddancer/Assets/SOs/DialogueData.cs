using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    //public string name; // *** here I might set up a condition dictionary
    [System.Serializable]
    public class DialogueUnit {
        [TextArea(3, 10)]
        public string sentence;
        public CameraToSwitch Camera;
        public AudioClip audioClipToPlay; // this might not work, the SO is not an object in the scene so
    }
    
    public DialogueUnit[] dialogueUnits;
    
    public string responseNo;
    public string responseYes;

    public enum EventsToCall { triggerNextDialogue, switchScene, customEvent }
    public EventsToCall NoEventToCall;
    public EventsToCall YesEventToCall;

    public enum CameraToSwitch {
        longCam,
        playerCamM, playerCamC,
        ravedemonCamM, ravedemonCamC,
        npc02CamM, npc02CamC,
        npc03CamM, npc03CamC,
        npc04CamM, npc04CamC,
        lastBuggedCamM, lastBuggedCamC
    }
    
}