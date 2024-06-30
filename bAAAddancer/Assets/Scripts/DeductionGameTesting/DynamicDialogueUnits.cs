using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DynamicDialogueUnits : MonoBehaviour
{
    [System.Serializable]
    public class DialogueUnit
    {
        public string dialogueText;
        public List<Criterion> speakerCriteria;
        public List<Criterion> spokenToCriteria;
        public UnityEvent onDialogueTriggered;

        public DialogueUnit(string dialogueText, List<Criterion> speakerCriteria, List<Criterion> spokenToCriteria)
        {
            this.dialogueText = dialogueText;
            this.speakerCriteria = speakerCriteria;
            this.spokenToCriteria = spokenToCriteria;
            this.onDialogueTriggered = new UnityEvent();
        }
    }

    public DialogueUnit[] sceneDialogueUnits;
}
