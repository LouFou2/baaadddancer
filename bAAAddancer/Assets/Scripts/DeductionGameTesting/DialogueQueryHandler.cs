using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogueQueryHandler : MonoBehaviour
{
    [SerializeField] private DialogueQueryCriteria[] queryCriteriaSOs;
    [SerializeField] private DynamicDialogueUnits[] dialogueUnits;
    [SerializeField] private CharacterStatsManager characterStatsManager;
    [SerializeField] private GameConditionsManager gameConditionsManager;

    [SerializeField] private CamDirector camDirector;

    [SerializeField] private DynamicDialogueUnits currentDialogueUnit;
    [SerializeField] private DialogueQueryCriteria currentQueryCriteria;
    private Queue<DialogueQueryCriteria.Query> queryQueue = new Queue<DialogueQueryCriteria.Query>();

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

    public enum DialogueState { PauseOrContinue, PlayerResponse }
    private DialogueState dialogueState;

    // Unity Events for responses
    public UnityEvent triggerNextDialogueEvent; // set the event in the inspector (what method to call)
    public UnityEvent switchSceneEvent;
    public UnityEvent customDialogueEvent; //etc

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

        button0Text = button0.GetComponentInChildren<TextMeshProUGUI>();
        button1Text = button1.GetComponentInChildren<TextMeshProUGUI>();

        playerIndex = characterStatsManager.playerIndex;

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
            queryQueue.Clear();
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
                    HandleNoResponse();
                }
                if (button1clicked)
                {
                    HandleYesResponse();
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
    private void InitializeQueryQueue()
    {
        foreach (var query in currentQueryCriteria.queries)
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
            HandlePlayerResponse();
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
                Debug.Log("Speakercritera:" + criterion.key + ": " + criterion.value);
            }

            List<int> matchingCharacterIndices = characterStatsManager.GetMatchingCharacterIndices(selectedCriteria);
            
            if (matchingCharacterIndices.Count > 0)
            {
                int selectedSpeakerIndex = matchingCharacterIndices[Random.Range(0, matchingCharacterIndices.Count)];

                previousSpeaker = currentSpeaker;
                currentSpeaker = selectedSpeakerIndex;
                Debug.Log("CURRENTSPEAKER: " + currentSpeaker);
                Debug.Log("PREVIOUS: " + previousSpeaker);

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

            // Trigger the UnityEvent
            selectedUnit.onDialogueTriggered.Invoke();

            PauseContinueResponse();
        }
    }
    private void UpdateSpeakerAndSpokenToDicts() 
    {
        // first reset the speaker stats
        for (int i = 0; i < 6; i++) 
        {
            if(i != currentSpeaker)
                characterStatsManager.ModifyCharacterStat(i, CharacterStat.CurrentSpeaker, 0);
            if (i != previousSpeaker)
                characterStatsManager.ModifyCharacterStat(i, CharacterStat.PreviousSpeaker, 0);
            if(i != currentSpokenTo)
                characterStatsManager.ModifyCharacterStat(i, CharacterStat.CurrentSpokenTo, 0);
            if(i != previousSpokenTo)
                characterStatsManager.ModifyCharacterStat(i, CharacterStat.PreviousSpokenTo, 0);
        }
        characterStatsManager.ModifyCharacterStat(currentSpeaker, CharacterStat.CurrentSpeaker, 1);
        characterStatsManager.ModifyCharacterStat(previousSpeaker, CharacterStat.PreviousSpeaker, 1);
        characterStatsManager.ModifyCharacterStat(currentSpokenTo, CharacterStat.CurrentSpokenTo, 1);
        characterStatsManager.ModifyCharacterStat(previousSpokenTo, CharacterStat.PreviousSpokenTo, 1);
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
    private void PauseContinueResponse() 
    {
        if (queryQueue.Count > 0)
        {
            dialogueState = DialogueState.PauseOrContinue;
            button1.gameObject.SetActive(true);
            button1.Select();
            button1Text.text = ">";
        }
        else 
        {
            HandlePlayerResponse();
        }
    }
    private void HandleCinematography(CameraDirections camera, CameraDirections distance, CameraDirections angle, CameraDirections zoom, CameraDirections shake)
    {
        camDirector.SetCameraState(camera, currentSpeaker, currentSpokenTo, distance, angle, zoom, shake);
    }
    private void HandlePlayerResponse() 
    {
        dialogueState = DialogueState.PlayerResponse;

        StartCoroutine(PlayerResponseCoroutine());

        /*button0Text.text = currentDialogueUnit.responseNo;
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
            currentDialogueUnit.playerCamShake);*/
    }
    private void HandleNoResponse() 
    {
        switch (currentDialogueUnit.NoEventToCall) 
        {
            case DynamicDialogueUnits.ResponseEvents.triggerNextDialogue:
                triggerNextDialogueEvent.Invoke();
                break;
            case DynamicDialogueUnits.ResponseEvents.switchScene:
                switchSceneEvent.Invoke();
                break;
            case DynamicDialogueUnits.ResponseEvents.customEvent:
                customDialogueEvent.Invoke();
                break;
        }
    }
    private void HandleYesResponse() 
    {
        switch (currentDialogueUnit.YesEventToCall)
        {
            case DynamicDialogueUnits.ResponseEvents.triggerNextDialogue:
                triggerNextDialogueEvent.Invoke();
                break;
            case DynamicDialogueUnits.ResponseEvents.switchScene:
                switchSceneEvent.Invoke();
                break;
            case DynamicDialogueUnits.ResponseEvents.customEvent:
                customDialogueEvent.Invoke();
                break;
        }
    }

    private IEnumerator PlayerResponseCoroutine()
    {
        // Pause for a bit before changing buttons and cinematography
        yield return new WaitForSeconds(2f); // Adjust the duration as needed

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
    }
}
