using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "CharacterData")]
public class CharacterData : ScriptableObject
{
    public enum CharacterRole { NPC, Demon, Player }
    public CharacterRole characterRoleSelect;

    //[Range(0f, 1f)] public float infectionLevel = 0f;
    [Range(0, 16)] public int infectionLevel = 0;

    public bool wasDebuggedLastRound;
    public bool lastCursedCharacter;
    public bool wasEliminated;
}
