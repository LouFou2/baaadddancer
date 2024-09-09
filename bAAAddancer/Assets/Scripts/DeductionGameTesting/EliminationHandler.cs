using UnityEngine;

public class EliminationHandler : MonoBehaviour
{
    private CharacterStatsManager charStatsManager;
    private GameConditionsManager gameConditionsManager;
    private void Start()
    {
        charStatsManager = FindObjectOfType<CharacterStatsManager>();
        gameConditionsManager = FindObjectOfType<GameConditionsManager>();

    }
    public void HandleElimination()
    {
        bool shouldEliminate = false;
        if (charStatsManager != null)
        {
            shouldEliminate = charStatsManager.ShouldEliminationHappen();
        }
        else
            Debug.LogWarning("couldn't reference CharacterStatsManager for handling elimination");

        if (shouldEliminate) 
        {
            gameConditionsManager.SetGameCondition(GameCondition.EliminationCalled, 1);
            Debug.Log("Elimination is Called");
        }
        else
        {
            Debug.Log("Skipped Elimination");
        }
    }
}
