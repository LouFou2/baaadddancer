using System.Collections.Generic;

using UnityEngine;

public class GameConditionsManager : MonoBehaviour
{
    public Dictionary<GameCondition, int> gameConditions = new Dictionary<GameCondition, int>();

    public void Start()
    {
        SetGameCondition(GameCondition.NewGame, 0); // need more sophisticated logic so this doesnt reset every new scene
        SetGameCondition(GameCondition.DialogueLine, 0);
    }

    public void GameStarted() 
    {
        SetGameCondition(GameCondition.NewGame, 1);
    } 
    
    public int GetGameCondition(GameCondition gameCondition)
    {
        return gameConditions.ContainsKey(gameCondition) ? gameConditions[gameCondition] : 0;
    }

    // Methods to modify game conditions and events
    public void SetGameCondition(GameCondition condition, int value)
    {
        gameConditions[condition] = value;
    }

    public void IncrementDialogueLine() 
    {
        int currentLine = GetGameCondition(GameCondition.DialogueLine);
        currentLine += 1;
        SetGameCondition(GameCondition.DialogueLine, currentLine);
    }
}
