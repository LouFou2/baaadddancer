using UnityEngine;

public class CharacterProfile : MonoBehaviour
{
    public CharacterData characterDataSO;

    public bool isPlayer = false;
    public bool isNPC = false;
    public bool isBug = false;

    private void Update()
    {
        if (characterDataSO != null) 
        {
            isPlayer = (characterDataSO.characterRoleSelect == CharacterData.CharacterRole.Player) ? true : false;
            isNPC = (characterDataSO.characterRoleSelect == CharacterData.CharacterRole.NPC) ? true : false;
            isBug = (characterDataSO.characterRoleSelect == CharacterData.CharacterRole.Bug) ? true : false;
        }
    }
    public void SetCharacterAsPlayer()
    {
        isPlayer = true;
        characterDataSO.characterRoleSelect = CharacterData.CharacterRole.Player;
    }
    public void SetCharacterAsNPC()
    {
        isNPC = true;
        characterDataSO.characterRoleSelect = CharacterData.CharacterRole.NPC;
    }
    public void SetCharacterAsBug()
    {
        isBug = true;
        characterDataSO.characterRoleSelect = CharacterData.CharacterRole.Bug;
    }
}
