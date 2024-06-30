using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager2 : MonoBehaviour
{
    [SerializeField] private DynamicDialogueUnits dialogueUnitsScript;
    [SerializeField] private CharacterManager2 characterManager;
    [SerializeField] private DialogueQueryCriteria queryCriteriaScriptableObject;

    public TextMeshProUGUI[] characterTextDisplay; // inject in inspector, make sure indexes match character indexes

    private Queue<DialogueQueryCriteria.Query> queryQueue = new Queue<DialogueQueryCriteria.Query>();

    private void Start()
    {
        InitializeQueryQueue();
        RunNextQuery();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RunNextQuery();
        }
    }

    private void InitializeQueryQueue()
    {
        foreach (var query in queryCriteriaScriptableObject.queries)
        {
            queryQueue.Enqueue(query);
        }
    }

    private void RunNextQuery()
    {
        if (queryQueue.Count > 0)
        {
            var query = queryQueue.Dequeue();
            RunQuery(query);
        }
        else
        {
            Debug.Log("No more queries in the queue.");
        }
    }
    public void RunQueryByIdentifier(string identifier)
    {
        var query = queryCriteriaScriptableObject.queries.Find(q => q.identifier == identifier);
        if (query != null)
        {
            RunQuery(query);
        }
        else
        {
            Debug.LogWarning($"Query with identifier {identifier} not found.");
        }
    }
    public void RunQuery(DialogueQueryCriteria.Query query)
    {
        Dictionary<CharacterStat, int> speakerQueryCriteria = new Dictionary<CharacterStat, int>();
        foreach (var criterion in query.speakerCriteria)
        {
            speakerQueryCriteria.Add(criterion.key, criterion.value);
        }

        DialogueQuery(speakerQueryCriteria);
    }

    public void DialogueQuery(Dictionary<CharacterStat, int> speakerQueryCriteria)
    {
        List<DynamicDialogueUnits.DialogueUnit> speakerMatchingUnits = new List<DynamicDialogueUnits.DialogueUnit>();
        List<DynamicDialogueUnits.DialogueUnit> spokenToMatchingUnits = new List<DynamicDialogueUnits.DialogueUnit>();

        // Find all matching units
        foreach (var unit in dialogueUnitsScript.sceneDialogueUnits)
        {
            if (IsMatch(unit, speakerQueryCriteria))
            {
                speakerMatchingUnits.Add(unit);
            }
        }

        // Sort matching units by score (number of matched criteria)
        speakerMatchingUnits.Sort((a, b) => b.speakerCriteria.Count.CompareTo(a.speakerCriteria.Count));

        // Select highest scoring matches
        List<DynamicDialogueUnits.DialogueUnit> highestScoringMatches = new List<DynamicDialogueUnits.DialogueUnit>();
        if (speakerMatchingUnits.Count > 0)
        {
            int highestScore = speakerMatchingUnits[0].speakerCriteria.Count;
            foreach (var unit in speakerMatchingUnits)
            {
                if (unit.speakerCriteria.Count == highestScore)
                {
                    highestScoringMatches.Add(unit);
                }
                else
                {
                    break; // Exit loop when score decreases
                }
            }
        }

        // Randomly choose one from highest scoring matches
        if (highestScoringMatches.Count > 0)
        {
            DynamicDialogueUnits.DialogueUnit selectedUnit = highestScoringMatches[Random.Range(0, highestScoringMatches.Count)];

            // Find a character that matches the selected unit's criteria
            Dictionary<CharacterStat, int> selectedCriteria = new Dictionary<CharacterStat, int>();
            foreach (var criterion in selectedUnit.speakerCriteria)
            {
                selectedCriteria.Add(criterion.key, criterion.value);
            }

            List<int> matchingCharacterIndices = characterManager.GetMatchingCharacterIndices(selectedCriteria);

            if (matchingCharacterIndices.Count > 0)
            {
                int selectedCharacterIndex = matchingCharacterIndices[Random.Range(0, matchingCharacterIndices.Count)];

                if (selectedCharacterIndex >= 0 && selectedCharacterIndex < characterTextDisplay.Length)
                {
                    characterTextDisplay[selectedCharacterIndex].text = selectedUnit.dialogueText;
                }
                else
                {
                    Debug.LogWarning("Invalid character index: " + selectedCharacterIndex);
                }

                // Trigger the UnityEvent
                selectedUnit.onDialogueTriggered.Invoke();
            }
        }
    }

    private bool IsMatch(DynamicDialogueUnits.DialogueUnit unit, Dictionary<CharacterStat, int> queryCriteria)
    {
        foreach (var criterion in queryCriteria)
        {
            bool criterionMatch = false;
            foreach (var unitCriterion in unit.speakerCriteria)
            {
                if (unitCriterion.key == criterion.Key && unitCriterion.value == criterion.Value)
                {
                    criterionMatch = true;
                    break;
                }
            }
            if (!criterionMatch)
            {
                return false;
            }
        }
        return true;
    }
}
