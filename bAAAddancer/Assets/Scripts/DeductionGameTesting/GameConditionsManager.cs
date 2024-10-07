using System.Collections.Generic;
using UnityEngine;

public class GameConditionsManager : MonoBehaviour
{
    public GameConditionsSO gameConditionsSO;
    public Dictionary<GameCondition, int> gameConditions = new Dictionary<GameCondition, int>();
    public CharacterStatsManager charStatsManager;

    public void Start()
    {
        charStatsManager = FindObjectOfType<CharacterStatsManager>();

        // first dialogue scene, reset ints
        if (GameManager.Instance.GetCurrentLevelKey() == LevelKey.IntroDialogue)
        {
            gameConditionsSO.GitGudMembersInt = 0;
            gameConditionsSO.GitCursedMembersInt = 0;
            gameConditionsSO.TeamCursedAmountInt = 0;
            gameConditionsSO.PlayerChooseGudInt = -1; // 0 = player choose cursed, 1 = player choose gud
        }

        SetGameCondition(GameCondition.DialogueLine, 0);
        SetGameCondition(GameCondition.DeceptionDetected, 0);
        SetGameCondition(GameCondition.TeamCursedAmount, gameConditionsSO.TeamCursedAmountInt);
        SetGameCondition(GameCondition.EliminationCalled, 0);

        SetGameCondition(GameCondition.GudBeatsCursed, DoesGudBeatCursed());
        SetGameCondition(GameCondition.GitGudMembersCount, gameConditionsSO.GitGudMembersInt);
        SetGameCondition(GameCondition.GitCursedMembersCount, gameConditionsSO.GitCursedMembersInt);
        SetGameCondition(GameCondition.PlayerChooseGud, gameConditionsSO.PlayerChooseGudInt);
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

    public void CountTeamCursedAmount() 
    {
        gameConditionsSO.TeamCursedAmountInt = 0;
        foreach (CharacterStatsSO charStat in charStatsManager.characterStats)
        {
            if (charStat.CursedInt == 1 && charStat.EliminatedInt == 0)
            {
                gameConditionsSO.TeamCursedAmountInt += 1;
            }
            
        }   
        SetGameCondition(GameCondition.TeamCursedAmount, gameConditionsSO.TeamCursedAmountInt);
    }

    public void CountGudVsCursedMembers()
    {
        gameConditionsSO.GitGudMembersInt = 0;
        gameConditionsSO.GitCursedMembersInt = 0;
        foreach (CharacterStatsSO charStat in charStatsManager.characterStats)
        {
            if (charStat.SideGudInt == 1)
            {
                gameConditionsSO.GitGudMembersInt += 1;
            }
            if (charStat.SideCursedInt == 1)
            {
                gameConditionsSO.GitCursedMembersInt += 1;
            }
        }
        SetGameCondition(GameCondition.GudBeatsCursed, DoesGudBeatCursed());
    }

    public int DoesGudBeatCursed()
    {
        if(gameConditionsSO.GitGudMembersInt > gameConditionsSO.GitCursedMembersInt)
            return 1;
        if (gameConditionsSO.GitGudMembersInt < gameConditionsSO.GitCursedMembersInt)
            return 0;
        else
            return -1; // if they are equal, default to -1
    }

    public void PlayerChooseGud()
    {
        gameConditionsSO.PlayerChooseGudInt = 1;
        SetGameCondition(GameCondition.PlayerChooseGud, 1);
    }
    public void PlayerChooseCurse()
    {
        gameConditionsSO.PlayerChooseGudInt = 0;
        SetGameCondition(GameCondition.PlayerChooseGud, 0);
    }
}
