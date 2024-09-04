using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;

public class DialogueQueryHandler : MonoBehaviour
{
    private DialogueSwitcher2 dialogueSwitcher;

    [SerializeField] private CharacterStatsManager characterStatsManager;
    [SerializeField] private GameConditionsManager gameConditionsManager;

    private DialogueEventsManager dialogueEventsManager;

    [SerializeField] private CamDirector camDirector;

    [SerializeField] private DynamicDialogueUnits currentDialogueUnit;
    [SerializeField] private DialogueQueryCriteria currentQueryCriteria;
    private Queue<DialogueQueryCriteria.Query> queryQueue = new Queue<DialogueQueryCriteria.Query>();
    private Queue<DialogueQueryCriteria.Query> responseQueryQueue = new Queue<DialogueQueryCriteria.Query>();

    private HashSet<DynamicDialogueUnits.DialogueUnit> usedDialogueUnits = new HashSet<DynamicDialogueUnits.DialogueUnit>();

    [SerializeField] private TextMeshProUGUI characterTextDisplay; // inject in inspector, make sure indexes match character indexes
    [SerializeField] private Button button0;
    [SerializeField] private Button button1;
    private TextMeshProUGUI button0Text;
    private TextMeshProUGUI button1Text;
    private bool button0clicked;
    private bool button1clicked;

    public int currentSpeaker { get; private set; }
    public int previousSpeaker { get; private set; }
    public int currentSpokenTo { get; private set; }
    public int previousSpokenTo { get; private set; }

    public int playerIndex;

    public enum DialogueState { PauseOrContinue, LastPause, PlayerResponse }
    private DialogueState dialogueState;

    /*
    // Unity Events for responses  //*** Maybe don't even need these eventually (since the events are set up in dialogue units)
    public UnityEvent triggerNextDialogueEvent; // set the event in the inspector (what method to call)
    public UnityEvent switchSceneEvent;
    public UnityEvent customDialogueEvent; //etc
    */

    private void Start()
    {
        dialogueSwitcher = GetComponent<DialogueSwitcher2>();
        dialogueEventsManager = GetComponent<DialogueEventsManager>();

        //currentDialogueUnit = dialogueSwitcher.currentDialogueUnits;
        
        //currentQueryCriteria = dialogueSwitcher.currentSceneQueryCriteria;

        button0Text = button0.GetComponentInChildren<TextMeshProUGUI>();
        button1Text = button1.GetComponentInChildren<TextMeshProUGUI>();

        playerIndex = characterStatsManager.playerIndex;
        if (playerIndex == -1) Debug.LogWarning("playerIndex not set properly");

        InitializeQueryQueue();
        RunNextQuery();
    }

    private void Update() // Update is mainly responsible for player UI interaction
    {
        // - Debugging Input options - 
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RunNextQuery();
        }
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            queryQueue.Clear(); //*** Might hold off on this so that it can first query WHICH player response to load
            HandlePlayerResponse();
        }

        // == Case Switching for button inputs == //
        switch (dialogueState)
        {
            case DialogueState.PauseOrContinue:
                if (button1clicked)
                {
                    RunNextQuery();
                }
                break;

            case DialogueState.PlayerResponse:
                if (button0clicked)
                {
                    dialogueEventsManager.HandleEvents(currentDialogueUnit.onPlayerRespondNo);
                }
                if (button1clicked)
                {
                    dialogueEventsManager.HandleEvents(currentDialogueUnit.onPlayerRespondYes);
                }
                break;

            default:
                dialogueState = DialogueState.PauseOrContinue;
                break;
        }
        button0clicked = false; // reset button click states
        button1clicked = false;
    }
    public void Button0Clicked()
    {
        button0clicked = true;
    }
    public void Button1Clicked()
    {
        button1clicked = true;
    }
    public void InitializeQueryQueue()
    {
        currentQueryCriteria = dialogueSwitcher.currentSceneQueryCriteria;
        currentDialogueUnit = dialogueSwitcher.currentDialogueUnits;

        foreach (var query in currentQueryCriteria.queries)
        {
            queryQueue.Enqueue(query);
        }
        foreach (var responseQuery in currentQueryCriteria.playerResponseQueries) 
        {
            responseQueryQueue.Enqueue(responseQuery);
        }
    }

    public void RunNextQuery()
    {
        if (queryQueue.Count > 0)
        {
            //Debug.Log(queryQueue.Count + " queries in queue");
            var query = queryQueue.Dequeue();
            RunQuery(query);
        }
        else
        {
            //Debug.Log("Player Response");
            RunNextResponseQuery();
            HandlePlayerResponse();
        }
    }

    //================================= *** call method above ***
    private void RunNextResponseQuery() // same as above, but only for player response
    {
        if (responseQueryQueue.Count > 0)
        {
            var responseQuery = responseQueryQueue.Dequeue();
            MatchPlayerResponse(responseQuery);
        }
        else
            Debug.Log("No more player response queries");
    } 
    //==================================

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
        // Is there generally a matching Speaker?
        if (!IsSpeakerMatch(query.speakerCriteria))
        {
            Debug.Log("No matching speaker, skipping to the next query.");
            RunNextQuery();
            return;
        }

        button0.gameObject.SetActive(false);
        button0Text.text = string.Empty;

        button1.gameObject.SetActive(false);
        button1Text.text = string.Empty;

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
        foreach (var unit in currentDialogueUnit.sceneDialogueUnits)
        {
            if (IsGameCriteriaMatchInUnit(unit, gameQueryCriteria) && !usedDialogueUnits.Contains(unit) && IsSpeakerCriteriaMatchInUnit(unit, speakerQueryCriteria))
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

            // == SPEAKER == //
            // Find the speaker character that matches the selected unit's criteria
            Dictionary<CharacterStat, int> selectedCriteria = new Dictionary<CharacterStat, int>();
            foreach (var criterion in selectedUnit.speakerCriteria)
            {
                selectedCriteria.Add(criterion.key, criterion.value);
                //Debug.Log("Speakercritera:" + criterion.key + ": " + criterion.value);
            }

            List<int> matchingCharacterIndices = characterStatsManager.GetMatchingCharacterIndices(selectedCriteria);
            
            if (matchingCharacterIndices.Count > 0)
            {
                int selectedSpeakerIndex = matchingCharacterIndices[Random.Range(0, matchingCharacterIndices.Count)];

                previousSpeaker = currentSpeaker;
                currentSpeaker = selectedSpeakerIndex;
                //Debug.Log("CURRENTSPEAKER: " + currentSpeaker);
                //Debug.Log("PREVIOUS: " + previousSpeaker);

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

            List<int> matchingSpokenToIndices = (spokenToCriteria.Count > 0) ? characterStatsManager.GetMatchingCharacterIndices(spokenToCriteria) : new List<int>(); 

            if (matchingSpokenToIndices.Count > 0)
            {
                int selectedSpokenToIndex = matchingSpokenToIndices[Random.Range(0, matchingSpokenToIndices.Count)];
                previousSpokenTo = currentSpokenTo;
                currentSpokenTo = selectedSpokenToIndex;
            }

            UpdateSpeakerAndSpokenToDicts();

            // DISPLAY TEXT *** REPLACE THIS WITH >> DialoguePlayer to play text...
            characterTextDisplay.transform.parent.gameObject.SetActive(true);
            characterTextDisplay.text = selectedUnit.dialogueText;

            // Increment Dialogue line
            gameConditionsManager.IncrementDialogueLine();

            // CINEMATOGRAPHY
            HandleCinematography(selectedUnit.camera, selectedUnit.distance, selectedUnit.angle, selectedUnit.zoom, selectedUnit.shake);

            // Trigger Dialogue Event
            dialogueEventsManager.HandleEvents(selectedUnit.onDialogueTriggered);

            HandlePauseContinue();
        }
    }
    
    private bool IsGameMatch(List<GameCriterion> gameQueryCriteria)
    {
        foreach (var criterion in gameQueryCriteria)
        {
            if (gameConditionsManager.GetGameCondition(criterion.key) != criterion.value)
            {
                Debug.Log($"Game Condition {criterion.key} is not {criterion.value}");
                return false; // Exit immediately if any game condition does not match
            }
        }
        return true;
    }
    private bool IsSpeakerMatch(List<CharCriterion> speakerQueryCriteria)
    {
        List<List<int>> allMatchingIndices = new List<List<int>>(); // A list of lists... *

        for (int i = 0; i < speakerQueryCriteria.Count; i++) 
        {
            var criteriaDictionary = new Dictionary<CharacterStat, int>
            {
                { speakerQueryCriteria[i].key, speakerQueryCriteria[i].value }
            };
            //Debug.Log($"checking speakercriteria{i} K: {speakerQueryCriteria[i].key} V: {speakerQueryCriteria[i].value}");
            List<int> matchingSpeakersIndices = characterStatsManager.GetMatchingCharacterIndices(criteriaDictionary);

            if (matchingSpeakersIndices.Count == 0)
            {
                //Debug.Log("No matching index found.");
                return false;                           // exit early if criteria finds no match
            }

           // Debug.Log($"potential matches for criterion {i}");
            allMatchingIndices.Add(matchingSpeakersIndices);
        }
        // Find the common indices that are present in all lists
        List<int> commonIndices = new List<int>(allMatchingIndices[0]); //starts with the first list in our list of lists *

        for (int i = 1; i < allMatchingIndices.Count; i++)
        {
            commonIndices = commonIndices.Intersect(allMatchingIndices[i]).ToList();
            if (commonIndices.Count == 0)
            {
                //Debug.Log("No common index found across all criteria.");
                return false; // Return false if no index is common across all criteria
            }
        }
        // If there are common indices, it means we have a speaker that matches all criteria
        if (commonIndices.Count > 0)
        {
            //Debug.Log("Found common index: " + commonIndices[0]);
            return true; // Return true if there is at least one common index
        }

        return false; // Return false if no common indices found
    }

    private bool IsSpeakerCriteriaMatchInUnit(DynamicDialogueUnits.DialogueUnit unit, Dictionary<CharacterStat, int> speakerQueryCriteria)
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

    private bool IsSpeakerMatchInResponseUnit(DynamicDialogueUnits.PlayerResponseUnit unit, Dictionary<CharacterStat, int> speakerQueryCriteria)
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

    private bool IsGameMatchInResponseUnit(DynamicDialogueUnits.PlayerResponseUnit unit, Dictionary<GameCondition, int> gameQueryCriteria)
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

    private void UpdateSpeakerAndSpokenToDicts()
    {
        // first reset the speaker stats
        for (int i = 0; i < 6; i++)
        {
            if (i != currentSpeaker)
                characterStatsManager.ModifyCharacterStat(i, CharacterStat.LastSpeaker, 0);
            if (i != currentSpokenTo)
                characterStatsManager.ModifyCharacterStat(i, CharacterStat.LastSpokenTo, 0);
        }
        characterStatsManager.ModifyCharacterStat(currentSpeaker, CharacterStat.LastSpeaker, 1);
        characterStatsManager.ModifyCharacterStat(currentSpokenTo, CharacterStat.LastSpokenTo, 1);
    }
    
    private void HandlePauseContinue() 
    {
        dialogueState = DialogueState.PauseOrContinue;
        button1.gameObject.SetActive(true);
        button1.Select();
        button1Text.text = ">";
        
    }
    private void MatchPlayerResponse(DialogueQueryCriteria.Query responseQuery) 
    {
        // Do game conditions exist for this response?
        if (!IsGameMatch(responseQuery.gameCriteria))
        {
            Debug.Log("Game conditions not matching for player response, skipping to the next query.");
            RunNextResponseQuery();
            return;
        }
        // Do speaker conditions exist for this response?
        if (!IsSpeakerMatch(responseQuery.speakerCriteria))
        {
            Debug.Log("Speaker conditions not matching for player response, skipping to the next query.");
            RunNextResponseQuery();
            return;
        }
        // If above checks pass
        // We can find a match in the dialogue unit:
        DynamicDialogueUnits.PlayerResponseUnit matchingResponseUnit = null;

        Dictionary<CharacterStat, int> speakerQueryCriteria = new Dictionary<CharacterStat, int>();
        foreach (var criterion in responseQuery.speakerCriteria)
        {
            speakerQueryCriteria.Add(criterion.key, criterion.value);
        }

        Dictionary<GameCondition, int> gameQueryCriteria = new Dictionary<GameCondition, int>();
        foreach (var criterion in responseQuery.gameCriteria)
        {
            gameQueryCriteria.Add(criterion.key, criterion.value);
        }

        foreach (var responseUnit in currentDialogueUnit.playerResponseUnits)
        {
            if (IsGameMatchInResponseUnit(responseUnit, gameQueryCriteria) && IsSpeakerMatchInResponseUnit(responseUnit, speakerQueryCriteria))
            {
                matchingResponseUnit = responseUnit;
            }
        }
        if (matchingResponseUnit == null)
        {
            RunNextResponseQuery();
        }
        else // We can modify the player response based on the query match
        {
            currentDialogueUnit.responseNo = matchingResponseUnit.responseNo;
            currentDialogueUnit.responseYes = matchingResponseUnit.responseYes;

            currentDialogueUnit.onPlayerRespondNo = matchingResponseUnit.onPlayerRespondNo;
            currentDialogueUnit.onPlayerRespondYes = matchingResponseUnit.onPlayerRespondYes;

            // ... add more modifications as needed
        }

    }
    
    private void HandlePlayerResponse() 
    {
        dialogueState = DialogueState.PlayerResponse;
        characterTextDisplay.transform.parent.gameObject.SetActive(false);

        button0Text.text = currentDialogueUnit.responseNo;
        button1Text.text = currentDialogueUnit.responseYes;

        button1.gameObject.SetActive(true);
        button1.Select();
        button0.gameObject.SetActive((string.IsNullOrEmpty(button0Text.text)) ? false : true); // button0 only active if it has text

        previousSpeaker = currentSpeaker;
        currentSpeaker = playerIndex;

        UpdateSpeakerAndSpokenToDicts();

        HandleCinematography
            (currentDialogueUnit.playerCamera,
            currentDialogueUnit.playerCamDistance,
            currentDialogueUnit.playerCamAngle,
            currentDialogueUnit.playerCamZoom,
            currentDialogueUnit.playerCamShake);

        //characterStatsManager.StoreCharacterStats();
    }
    
    private void HandleCinematography(CameraDirections camera, CameraDirections distance, CameraDirections angle, CameraDirections zoom, CameraDirections shake)
    {
        camDirector.SetCameraState(camera, currentSpeaker, currentSpokenTo, distance, angle, zoom, shake);
    }

}
