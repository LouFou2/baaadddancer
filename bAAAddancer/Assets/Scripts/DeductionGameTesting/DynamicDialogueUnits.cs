using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        public UnityEvent onDialogueTriggered;
    }

    //public LevelKey levelKey;
    public DialogueUnit[] sceneDialogueUnits;

    // player response
    public string responseNo;
    public string responseYes;
    public CharCriterion spokenToCriterion;
    public CameraDirections playerCamera, playerCamDistance, playerCamAngle, playerCamZoom, playerCamShake;
    public UnityEvent onPlayerRespondNo;
    public UnityEvent onPlayerRespondYes;
}
