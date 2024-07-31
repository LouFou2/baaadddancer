using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueQueryCriteria", menuName = "ScriptableObjects/DialogueQueryCriteria", order = 1)]
public class DialogueQueryCriteria : ScriptableObject
{
    [System.Serializable]
    public class Query
    {
        public string identifier; // Unique identifier for the query
        public List<CharCriterion> speakerCriteria = new List<CharCriterion>(); // Criteria for selecting the speaker
        public List<GameCriterion> gameCriteria = new List<GameCriterion>(); // New criteria for game conditions
    }

    public List<Query> queries = new List<Query>();
}

