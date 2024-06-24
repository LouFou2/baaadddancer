using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager2 : MonoBehaviour
{
    [SerializeField] private DynamicDialogueUnits dialogueUnitsScript;
    [SerializeField] private CharacterManager2 characterManager;

    public TextMeshProUGUI[] characterTextDisplay; // inject in inspector, make sure indexes match character indexes

    private void Start()
    {
        // Example criteria to query dialogue units
        Dictionary<string, int> queryCriteria = new Dictionary<string, int>
        {
            { "Who", 0 }
        };
        DialogueQuery(queryCriteria);
    }

    public void DialogueQuery(Dictionary<string, int> queryCriteria)
    {
        List<DynamicDialogueUnits.DialogueUnit> matchingUnits = new List<DynamicDialogueUnits.DialogueUnit>();

        // Find all matching units
        foreach (var unit in dialogueUnitsScript.sceneDialogueUnits)
        {
            if (IsMatch(unit, queryCriteria))
            {
                matchingUnits.Add(unit);
            }
        }

        // Sort matching units by score (number of matched criteria)
        matchingUnits.Sort((a, b) => b.criteria.Count.CompareTo(a.criteria.Count));

        // Select highest scoring matches
        List<DynamicDialogueUnits.DialogueUnit> highestScoringMatches = new List<DynamicDialogueUnits.DialogueUnit>();
        if (matchingUnits.Count > 0)
        {
            int highestScore = matchingUnits[0].criteria.Count;
            foreach (var unit in matchingUnits)
            {
                if (unit.criteria.Count == highestScore)
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

            // Display dialogue text on the character's text display
            int characterIndex = queryCriteria["Who"];
            if (characterIndex >= 0 && characterIndex < characterTextDisplay.Length)
            {
                characterTextDisplay[characterIndex].text = selectedUnit.dialogueText;
            }
            else
            {
                Debug.LogWarning("Invalid character index: " + characterIndex);
            }
        }
    }

    private bool IsMatch(DynamicDialogueUnits.DialogueUnit unit, Dictionary<string, int> queryCriteria)
    {
        foreach (var criterion in queryCriteria)
        {
            bool criterionMatch = false;
            foreach (var unitCriterion in unit.criteria)
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
