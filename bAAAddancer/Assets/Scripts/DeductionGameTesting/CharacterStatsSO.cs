using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStatsSO", menuName = "CharacterStats/CharacterStatsSO")]
public class CharacterStatsSO : ScriptableObject
{
    public int IsPlayerInt;
    public int IsDemonInt;
    public int CursedInt;
    public int LastCursedInt;
    public int InfluenceInt;
    public int PerceptionInt;
    public int DeceptionInt;
    public int EliminatorInt;
    public int VoteTargetInt;
    public int EliminationVoteCountsInt;
    public int EliminatedInt;
    public int SideGudInt;
    public int SideCursedInt;
    public int NpcIndexInt;
}
