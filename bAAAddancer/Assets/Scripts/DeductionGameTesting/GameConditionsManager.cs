using System.Collections.Generic;

using UnityEngine;

public class GameConditionsManager : MonoBehaviour
{
    public Dictionary<GameCondition, int> gameConditions = new Dictionary<GameCondition, int>();

    public void Start()
    {
        SetGameCondition(GameCondition.NewGame, 0); // need more sophisticated logic so this doesnt reset every new scene
        SetGameCondition(GameCondition.DialogueLine, 0);
        int startNewGame = GetGameCondition(GameCondition.NewGame);
        int startDialogueLine = GetGameCondition(GameCondition.DialogueLine);
        Debug.Log("New Game:" + startNewGame);
        Debug.Log("Dialogue line: " + startDialogueLine);
    }

    public void GameStart() 
    {
        SetGameCondition(GameCondition.NewGame, 1);
    } 
    // Methods to modify game conditions and events
    public void SetGameCondition(GameCondition condition, int value)
    {
        gameConditions[condition] = value;
    }

    public int GetGameCondition(GameCondition gameCondition)
    {
        return gameConditions.ContainsKey(gameCondition) ? gameConditions[gameCondition] : 0;
    }

    public void IncrementDialogueLine() 
    {
        int currentLine = GetGameCondition(GameCondition.DialogueLine);
        currentLine += 1;
        SetGameCondition(GameCondition.DialogueLine, currentLine);
        Debug.Log("Dialogue line: " + currentLine);
    }
}
