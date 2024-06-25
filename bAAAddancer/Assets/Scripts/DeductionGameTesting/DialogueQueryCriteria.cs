using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueQueryCriteria", menuName = "ScriptableObjects/DialogueQueryCriteria", order = 1)]
public class DialogueQueryCriteria : ScriptableObject
{
    [System.Serializable]
    public class Criterion
    {
        public CharacterStat key;
        public int value;
    }

    [System.Serializable]
    public class Query
    {
        public string identifier; // Unique identifier for the query
        public List<Criterion> criteria = new List<Criterion>();
    }

    public List<Query> queries = new List<Query>();
}

