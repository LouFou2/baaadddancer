using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNew Dialogue", menuName = "NewDialogueSystem/Dialogue Data")]
public class DynamicDialogueUnits : ScriptableObject
{
    [System.Serializable]
    public class DialogueUnit
    {
        [TextArea(3, 10)]
        public string dialogueText;
        public List<GameCriterion> gameCriteria;
        public List<CharCriterion> speakerCriteria;
        public List<CharCriterion> spokenToCriteria;
        [Tooltip("Camera Directions")]
        public CameraDirections camera, distance, angle, zoom, shake;
        public DialogueEventData[] onDialogueTriggered;
    }

    public DialogueUnit[] sceneDialogueUnits;
    public PlayerResponseUnit[] playerResponseUnits;

    // player response
    public string responseNo;
    public string responseYes;
    public CharCriterion spokenToCriterion;
    public CameraDirections playerCamera, playerCamDistance, playerCamAngle, playerCamZoom, playerCamShake;
    public DialogueEventData[] onPlayerRespondNo;
    public DialogueEventData[] onPlayerRespondYes;

    [System.Serializable]
    public class PlayerResponseUnit 
    {
        public List<GameCriterion> gameCriteria;
        public List<CharCriterion> speakerCriteria;
        public string responseNo;
        public string responseYes;
        public CharCriterion spokenToCriterion;
        public CameraDirections playerCamera, playerCamDistance, playerCamAngle, playerCamZoom, playerCamShake;
        public DialogueEventData[] onPlayerRespondNo;
        public DialogueEventData[] onPlayerRespondYes;
    }
}
