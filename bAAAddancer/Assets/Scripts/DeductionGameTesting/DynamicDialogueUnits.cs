using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicDialogueUnits : MonoBehaviour
{
    [System.Serializable]
    public class Criterion
    {
        public string key;
        public int value;
    }

    [System.Serializable]
    public class DialogueUnit
    {
        public string dialogueText;
        public List<Criterion> criteria;

        public DialogueUnit(string dialogueText, List<Criterion> criteria)
        {
            this.dialogueText = dialogueText;
            this.criteria = criteria;
        }
    }

    public DialogueUnit[] sceneDialogueUnits;

}
