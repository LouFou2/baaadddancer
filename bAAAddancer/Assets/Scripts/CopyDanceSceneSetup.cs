using UnityEngine;

public class CopyDanceSceneSetup : MonoBehaviour
{
    private CharacterManager characterManager;
    [SerializeField] private GameObject playerSetupTransforms;
    [SerializeField] private GameObject[] npcSetupTransforms;
    void Start()
    {
        characterManager = FindObjectOfType<CharacterManager>();

        int npcIndex = 0;

        for (int i = 0; i < characterManager.characters.Length; i++)
        {
            GameObject character = characterManager.characters[i];
            
            CharacterProfile characterProfile = character.GetComponent<CharacterProfile>();
            CopyProxyRig copyProxyRig = character.GetComponent<CopyProxyRig>();
            CopyDance copyDanceScript = character.GetComponent<CopyDance>();

            character.SetActive(true);
            copyProxyRig.enabled = false;

            if (characterProfile.characterDataSO.characterRoleSelect == CharacterData.CharacterRole.Player)
            {
                copyDanceScript.enabled = false;
                character.transform.position = playerSetupTransforms.transform.position;
                character.transform.rotation = playerSetupTransforms.transform.rotation;
            }
            else
            {
                copyDanceScript.enabled = true;
                character.transform.position = npcSetupTransforms[npcIndex].transform.position;
                character.transform.rotation = npcSetupTransforms[npcIndex].transform.rotation;
                npcIndex++;
                //set character as left screen, center screen, or right screen:
                if(npcIndex < 3) // 3 is the center character
                    copyDanceScript.charLeftScreen = true;
                else if(npcIndex == 3)
                    copyDanceScript.charCenterScreen = true;
                else
                    copyDanceScript.charRightScreen = true;
            }
        }
    }

}
