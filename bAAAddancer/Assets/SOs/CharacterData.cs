using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "CharacterData")]
public class CharacterData : ScriptableObject
{
    public enum CharacterRole { NPC, Demon, Player }
    public CharacterRole characterRoleSelect;

    // Alignment
    public enum CharacterAlignment { Gud1, Gud2, Neutral, Bent1, Bent2, Player }
    public CharacterAlignment charAlignment;

    // Speakers
    public int speakerIndex;
    public int spokenToIndex;

    [Range(0f, 1f)] public float infectionLevel = 0f;

    public bool wasDebuggedLastRound;
    public bool lastCursedCharacter;

    public bool wasEliminated;
}
