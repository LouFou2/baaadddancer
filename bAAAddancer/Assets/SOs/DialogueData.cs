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
    //*** This bit will be replaced with new DialogueUnit above
    [TextArea(3, 10)]
    public string[] sentences;
    //***
    //Like this:
    public DialogueUnit[] dialogueUnits;
     /* 
        * which means all scripts referring to the old sentences array 
        * will have to get sentences from the DialogueUnit somehow
     */
    public string responseNo;
    public string responseYes;

    public enum EventsToCall { triggerNextDialogue, switchScene, customEvent }
    public EventsToCall NoEventToCall;
    public EventsToCall YesEventToCall;

    public enum CameraToSwitch {
        longCam,
        playerCamL, playerCamM, playerCamC,
        npc01CamL, npc01CamM, npc01CamC,
        npc02CamL, npc02CamM, npc02CamC,
        npc03CamL, npc03CamM, npc03CamC,
        npc04CamL, npc04CamM, npc04CamC,
        npc05CamL, npc05CamM, npc05CamC
    }
    
}