using System.Collections.Generic;
using UnityEngine;

public class GameConditionsManager : MonoBehaviour
{
    public Dictionary<GameCondition, int> gameConditions = new Dictionary<GameCondition, int>();
    //public int dialogueLine = -1;

    public void Start()
    {
        SetGameCondition(GameCondition.DialogueLine, 0);
        //dialogueLine = 0;
    }

    public int GetGameCondition(GameCondition gameCondition)
    {
        return gameConditions.ContainsKey(gameCondition) ? gameConditions[gameCondition] : 0;
    }

    // Methods to modify game conditions and events
    public void SetGameCondition(GameCondition condition, int value)
    {
        gameConditions[condition] = value;
        //Debug.Log("GameCondition " + condition + " set to value: " + value);
    }

    public void IncrementDialogueLine() 
    {
        int currentLine = GetGameCondition(GameCondition.DialogueLine);
        currentLine += 1;
        //dialogueLine = currentLine;
        SetGameCondition(GameCondition.DialogueLine, currentLine);
    }
}
