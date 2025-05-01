using UnityEngine;

public class CopyDanceSceneSetup : MonoBehaviour
{
    private CharacterManager characterManager;
    [SerializeField] private GameObject playerSetupTransforms;
    [SerializeField] private GameObject[] npcSetupTransforms;

    [SerializeField] private GameObject playerRaveSetupTransforms;
    [SerializeField] private GameObject[] npcRaveSetupTransforms;

    public bool charsPositionedInScene;

    void Start()
    {
        int roundIndex = GameManager.Instance.GetCurrentRound();

        charsPositionedInScene = false;

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
                copyDanceScript.enabled = true;

                if (roundIndex < 4)
                {
                    character.transform.position = playerSetupTransforms.transform.position;
                    character.transform.rotation = playerSetupTransforms.transform.rotation;
                }
                if (roundIndex == 4)
                {
                    character.transform.position = playerRaveSetupTransforms.transform.position;
                    character.transform.rotation = playerRaveSetupTransforms.transform.rotation;
                }

            }
            else
            {
                copyDanceScript.enabled = true;

                if (roundIndex < 4)
                {
                    character.transform.position = npcSetupTransforms[npcIndex].transform.position;
                    character.transform.rotation = npcSetupTransforms[npcIndex].transform.rotation;
                }
                if (roundIndex == 4)
                {
                    character.transform.position = npcRaveSetupTransforms[npcIndex].transform.position;
                    character.transform.rotation = npcRaveSetupTransforms[npcIndex].transform.rotation;
                }
                    

                npcIndex++;
            }
        }
        charsPositionedInScene = true;
    }

}
