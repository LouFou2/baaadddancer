using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DynamicDialogueUnits : MonoBehaviour
{
    [System.Serializable]
    public class DialogueUnit
    {
        public string dialogueText;
        public List<CharCriterion> speakerCriteria;
        public List<CharCriterion> spokenToCriteria;
        public List<GameCriterion> gameCriteria;
        public UnityEvent onDialogueTriggered;

        /*public DialogueUnit(string dialogueText, List<CharCriterion> speakerCriteria, List<CharCriterion> spokenToCriteria)
        {
            this.dialogueText = dialogueText;
            this.speakerCriteria = speakerCriteria;
            this.spokenToCriteria = spokenToCriteria;
            this.onDialogueTriggered = new UnityEvent();
        }*/
    }

    public DialogueUnit[] sceneDialogueUnits;
}
