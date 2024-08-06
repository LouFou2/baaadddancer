using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueQueryHandler : MonoBehaviour
{
    [SerializeField] private DynamicDialogueUnits dialogueUnitsScript;
    [SerializeField] private CharacterStatsManager characterManager;
    [SerializeField] private GameConditionsManager gameConditionsManager;
    [SerializeField] private DialogueQueryCriteria queryCriteriaScriptableObject;

    public TextMeshProUGUI[] characterTextDisplay; // inject in inspector, make sure indexes match character indexes

    private Queue<DialogueQueryCriteria.Query> queryQueue = new Queue<DialogueQueryCriteria.Query>();

    public int currentSpeaker { get; private set; }
    public int previousSpeaker { get; private set; }
    public int currentSpokenTo { get; private set; }
    public int previousSpokenTo { get; private set; }

    private HashSet<DynamicDialogueUnits.DialogueUnit> usedDialogueUnits = new HashSet<DynamicDialogueUnits.DialogueUnit>();


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
        // Check game conditions first)
        if (!IsGameMatch(query.gameCriteria))
        {
            Debug.Log("Game conditions do not match, skipping to the next query.");
            RunNextQuery();
            return;
        }

        Dictionary<CharacterStat, int> speakerQueryCriteria = new Dictionary<CharacterStat, int>();
        foreach (var criterion in query.speakerCriteria)
        {
            speakerQueryCriteria.Add(criterion.key, criterion.value);
        }

        Dictionary<GameCondition, int> gameQueryCriteria = new Dictionary<GameCondition, int>();
        foreach (var criterion in query.gameCriteria)
        {
            gameQueryCriteria.Add(criterion.key, criterion.value);
        }

        DialogueQuery(speakerQueryCriteria, gameQueryCriteria);
    }

    public void DialogueQuery(Dictionary<CharacterStat, int> speakerQueryCriteria, Dictionary<GameCondition, int> gameQueryCriteria)
    {
        List<DynamicDialogueUnits.DialogueUnit> speakerMatchingUnits = new List<DynamicDialogueUnits.DialogueUnit>();
        List<DynamicDialogueUnits.DialogueUnit> spokenToMatchingUnits = new List<DynamicDialogueUnits.DialogueUnit>();

        // Find all matching units that haven't been used yet
        foreach (var unit in dialogueUnitsScript.sceneDialogueUnits)
        {
            if (IsGameCriteriaMatchInUnit(unit, gameQueryCriteria) && !usedDialogueUnits.Contains(unit) && IsSpeakerMatch(unit, speakerQueryCriteria))
            {
                speakerMatchingUnits.Add(unit);
            }
        }
        if (speakerMatchingUnits.Count == 0) RunNextQuery();

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

            // Mark the selected unit as used
            usedDialogueUnits.Add(selectedUnit);

            // Find a character that matches the selected unit's criteria
            Dictionary<CharacterStat, int> selectedCriteria = new Dictionary<CharacterStat, int>();
            foreach (var criterion in selectedUnit.speakerCriteria)
            {
                selectedCriteria.Add(criterion.key, criterion.value);
            }

            List<int> matchingCharacterIndices = characterManager.GetMatchingCharacterIndices(selectedCriteria);

            if (matchingCharacterIndices.Count > 0)
            {
                int selectedSpeakerIndex = matchingCharacterIndices[Random.Range(0, matchingCharacterIndices.Count)];

                previousSpeaker = currentSpeaker;
                currentSpeaker = selectedSpeakerIndex;

                if (selectedSpeakerIndex >= 0 && selectedSpeakerIndex < characterTextDisplay.Length)
                {
                    characterTextDisplay[selectedSpeakerIndex].text = selectedUnit.dialogueText;

                    // Increment the SpokenAmount stat
                    var characterStats = characterManager.GetCharacterStats(selectedSpeakerIndex);
                    if (characterStats != null && characterStats.ContainsKey(CharacterStat.SpokenAmount))
                    {
                        characterManager.ModifyCharacterStat(selectedSpeakerIndex, CharacterStat.SpokenAmount, characterStats[CharacterStat.SpokenAmount] + 1);
                    }
                    else
                    {
                        characterManager.ModifyCharacterStat(selectedSpeakerIndex, CharacterStat.SpokenAmount, 1);
                    }
                }
                else
                {
                    Debug.LogWarning("Invalid character index: " + selectedSpeakerIndex);
                }

            }

            // Now find the spoken-to character based on the selected unit's spokenToCriteria
            Dictionary<CharacterStat, int> spokenToCriteria = new Dictionary<CharacterStat, int>();
            foreach (var criterion in selectedUnit.spokenToCriteria)
            {
                spokenToCriteria.Add(criterion.key, criterion.value);
            }

            List<int> matchingSpokenToIndices = characterManager.GetMatchingCharacterIndices(spokenToCriteria);

            if (matchingSpokenToIndices.Count > 0)
            {
                int selectedSpokenToIndex = matchingSpokenToIndices[Random.Range(0, matchingSpokenToIndices.Count)];
                previousSpokenTo = currentSpokenTo;
                currentSpokenTo = selectedSpokenToIndex;
            }

            // Trigger the UnityEvent
            selectedUnit.onDialogueTriggered.Invoke();
        }
    }

    private bool IsSpeakerMatch(DynamicDialogueUnits.DialogueUnit unit, Dictionary<CharacterStat, int> speakerQueryCriteria)
    {
        foreach (var criterion in speakerQueryCriteria)
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
    private bool IsGameMatch(List<GameCriterion> gameQueryCriteria)
    {
        foreach (var criterion in gameQueryCriteria)
        {
            if (gameConditionsManager.GetGameCondition(criterion.key) != criterion.value)
            {
                return false; // Exit immediately if any game condition does not match
            }
        }
        return true;
    }
    private bool IsGameCriteriaMatchInUnit(DynamicDialogueUnits.DialogueUnit unit, Dictionary<GameCondition, int> gameQueryCriteria)
    {
        foreach (var criterion in gameQueryCriteria)
        {
            bool criterionMatch = false;
            foreach (var unitCriterion in unit.gameCriteria)
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
