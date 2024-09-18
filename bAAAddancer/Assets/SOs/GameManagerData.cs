using UnityEngine;

[CreateAssetMenu(fileName = "GameManagerData", menuName = "Game/GameManagerData")]
public class GameManagerData : ScriptableObject
{
    public LevelKey currentLevelKey;
    public int currentRound;
}
