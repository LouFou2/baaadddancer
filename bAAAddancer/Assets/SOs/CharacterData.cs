using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "CharacterData")]
public class CharacterData : ScriptableObject
{
    public enum CharacterRole { Player, NPC, Bug }
    public CharacterRole characterRoleSelect;

    [Range(0f, 1f)] public float infectionLevel = 0f;
    [Range(0f, 1f)] public float spreadInfectionChance = 0f;
}
