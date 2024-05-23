using UnityEngine;

[CreateAssetMenu(fileName = "GameManagerData", menuName = "Game/GameManagerData")]
public class GameManagerData : ScriptableObject
{
    //public GameManager.LevelKey currentLevelKey;
    public LevelKey currentLevelKey;
    public int currentRound;
}
