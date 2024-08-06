using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DynamicDialogueUnits : MonoBehaviour
{
    [System.Serializable]
    public class DialogueUnit
    {
        [TextArea(3, 10)]
        public string dialogueText;
        public List<CharCriterion> speakerCriteria;
        public List<CharCriterion> spokenToCriteria;
        public List<GameCriterion> gameCriteria;
        public UnityEvent onDialogueTriggered;
    }

    public DialogueUnit[] sceneDialogueUnits;
}
