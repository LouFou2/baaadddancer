using System.Collections.Generic;
using UnityEngine;

public class GameConditionsManager : MonoBehaviour
{
    public Dictionary<GameCondition, int> gameConditions = new Dictionary<GameCondition, int>();

    public void Start()
    {
        SetGameCondition(GameCondition.DialogueLine, 0);
        SetGameCondition(GameCondition.DeceptionDetected, 0);
        SetGameCondition(GameCondition.TeamCursedAmount, 0); //*** MAKE SURE TO HANDLE THIS BETTER (not just reset in start)
        SetGameCondition(GameCondition.EliminationCalled, 0);
    }

    public int GetGameCondition(GameCondition gameCondition)
    {
        return gameConditions.ContainsKey(gameCondition) ? gameConditions[gameCondition] : 0;
    }

    // Methods to modify game conditions and events
    public void SetGameCondition(GameCondition condition, int value)
    {
        gameConditions[condition] = value;
        Debug.Log("GameCondition " + condition + " set to value: " + value);
    }

    public void IncrementDialogueLine() 
    {
        int currentLine = GetGameCondition(GameCondition.DialogueLine);
        currentLine += 1;
        SetGameCondition(GameCondition.DialogueLine, currentLine);
    }

    public void HandleLieDetection() 
    {
        Debug.Log("Lie Detected, Setting Game Condition");
        SetGameCondition(GameCondition.DeceptionDetected, 1);
    }
    public void ResetLieDetection()
    {
        SetGameCondition(GameCondition.DeceptionDetected, 0);
    }
    public void IncrementTeamCursedAmount() 
    {
        int teamCursedAmount = GetGameCondition(GameCondition.TeamCursedAmount);
        teamCursedAmount += 1;
        SetGameCondition(GameCondition.TeamCursedAmount, teamCursedAmount);
    }
}
