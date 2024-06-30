using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueQueryCriteria", menuName = "ScriptableObjects/DialogueQueryCriteria", order = 1)]
public class DialogueQueryCriteria : ScriptableObject
{
    [System.Serializable]
    public class Query
    {
        public string identifier; // Unique identifier for the query
        public List<Criterion> speakerCriteria = new List<Criterion>(); // Criteria for selecting the speaker
    }

    public List<Query> queries = new List<Query>();
}

