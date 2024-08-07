using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueQueryHandler : MonoBehaviour
{
    [SerializeField] private DialogueQueryCriteria[] queryCriteriaSOs;
    [SerializeField] private DynamicDialogueUnits[] dialogueUnits;
    [SerializeField] private CharacterStatsManager characterStatsManager;
    [SerializeField] private GameConditionsManager gameConditionsManager;

    [SerializeField] private DynamicDialogueUnits currentDialogueUnit;
    [SerializeField] private DialogueQueryCriteria currentQueryCriteria;

    public TextMeshProUGUI characterTextDisplay; // inject in inspector, make sure indexes match character indexes

    private Queue<DialogueQueryCriteria.Query> queryQueue = new Queue<DialogueQueryCriteria.Query>();

    public int currentSpeaker { get; private set; }
    public int previousSpeaker { get; private set; }
    public int currentSpokenTo { get; private set; }
    public int previousSpokenTo { get; private set; }

    private HashSet<DynamicDialogueUnits.DialogueUnit> usedDialogueUnits = new HashSet<DynamicDialogueUnits.DialogueUnit>();


    private void Start()
    {
        // Check LevelKey and assign current Dialogue Unit
        LevelKey levelKey = GameManager.Instance.GetCurrentLevelKey();
        foreach (DynamicDialogueUnits dialogueUnit in dialogueUnits) 
        {
            if (dialogueUnit.levelKey == levelKey) 
            {
                currentDialogueUnit = dialogueUnit;
            }
        }
        foreach (DialogueQueryCriteria queryCriteriaSOs in queryCriteriaSOs) 
        {
            if (queryCriteriaSOs.levelKey == levelKey) 
            {
                currentQueryCriteria = queryCriteriaSOs;
            }
        }
        
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
        int queryCount = 0;
        foreach (var query in currentQueryCriteria.queries)
        {
            queryQueue.Enqueue(query);
            queryCount++;
            Debug.Log("Queries in queue: " + queryCount);
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
        var query = currentQueryCriteria.queries.Find(q => q.identifier == identifier);
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
        Debug.Log("Running query...");
        List<DynamicDialogueUnits.DialogueUnit> speakerMatchingUnits = new List<DynamicDialogueUnits.DialogueUnit>();
        List<DynamicDialogueUnits.DialogueUnit> spokenToMatchingUnits = new List<DynamicDialogueUnits.DialogueUnit>();

        // Find all matching units that haven't been used yet
        foreach (var unit in currentDialogueUnit.sceneDialogueUnits)
        {
            if (IsGameCriteriaMatchInUnit(unit, gameQueryCriteria) && !usedDialogueUnits.Contains(unit) && IsSpeakerMatch(unit, speakerQueryCriteria))
            {
                speakerMatchingUnits.Add(unit);
                Debug.Log("Finding matching speakers");
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

            // == SPEAKER == //
            // Find the speaker character that matches the selected unit's criteria
            Dictionary<CharacterStat, int> selectedCriteria = new Dictionary<CharacterStat, int>();
            foreach (var criterion in selectedUnit.speakerCriteria)
            {
                selectedCriteria.Add(criterion.key, criterion.value);
            }

            List<int> matchingCharacterIndices = characterStatsManager.GetMatchingCharacterIndices(selectedCriteria);

            if (matchingCharacterIndices.Count > 0)
            {
                int selectedSpeakerIndex = matchingCharacterIndices[Random.Range(0, matchingCharacterIndices.Count)];

                previousSpeaker = currentSpeaker;
                currentSpeaker = selectedSpeakerIndex;
                characterStatsManager.ModifyCharacterStat(previousSpeaker, CharacterStat.CurrentSpeaker, 0); //first reset this stat on previous speaker
                characterStatsManager.ModifyCharacterStat(previousSpeaker, CharacterStat.PreviousSpeaker, 1);
                characterStatsManager.ModifyCharacterStat(currentSpeaker, CharacterStat.CurrentSpeaker, 1);

                // DISPLAY TEXT *** REPLACE THIS WITH >> DialoguePlayer to play text...
                characterTextDisplay.text = selectedUnit.dialogueText;
                Debug.Log("Text should display");

                // Increment Dialogue line
                gameConditionsManager.IncrementDialogueLine();

                // Increment the SpokenAmount stat
                var characterStats = characterStatsManager.GetCharacterStats(selectedSpeakerIndex);
                if (characterStats != null && characterStats.ContainsKey(CharacterStat.SpokenAmount))
                {
                    characterStatsManager.ModifyCharacterStat(selectedSpeakerIndex, CharacterStat.SpokenAmount, characterStats[CharacterStat.SpokenAmount] + 1);
                }
                else
                {
                    characterStatsManager.ModifyCharacterStat(selectedSpeakerIndex, CharacterStat.SpokenAmount, 1);
                }

            }

            // == SPOKEN TO == //
            // Now find the spoken-to character based on the selected unit's spokenToCriteria
            Dictionary<CharacterStat, int> spokenToCriteria = new Dictionary<CharacterStat, int>();
            foreach (var criterion in selectedUnit.spokenToCriteria)
            {
                spokenToCriteria.Add(criterion.key, criterion.value);
            }

            List<int> matchingSpokenToIndices = characterStatsManager.GetMatchingCharacterIndices(spokenToCriteria);

            if (matchingSpokenToIndices.Count > 0)
            {
                int selectedSpokenToIndex = matchingSpokenToIndices[Random.Range(0, matchingSpokenToIndices.Count)];
                previousSpokenTo = currentSpokenTo;
                currentSpokenTo = selectedSpokenToIndex;

                characterStatsManager.ModifyCharacterStat(previousSpokenTo, CharacterStat.CurrentSpokenTo, 0);
                characterStatsManager.ModifyCharacterStat(previousSpokenTo, CharacterStat.PreviousSpokenTo, 1);
                characterStatsManager.ModifyCharacterStat(currentSpokenTo, CharacterStat.CurrentSpokenTo, 1);
            }

            // Trigger the UnityEvent
            selectedUnit.onDialogueTriggered.Invoke();
        }
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
