using UnityEngine;
using UnityEngine.Events;

public class MakeDanceSceneSetup : MonoBehaviour
{
    private CharacterManager characterManager;
    private PlayerControls playerControls;
    private SceneSwitcher sceneSwitcher;

    public UnityEvent yButtonEndSceneEvent;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }
    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }
    void Start()
    {
        GameManager.Instance.NewRound();

        sceneSwitcher = FindObjectOfType<SceneSwitcher>();
        characterManager = FindObjectOfType<CharacterManager>();

        foreach (GameObject character in characterManager.characters) 
        {
            CharacterProfile characterProfile = character.GetComponent<CharacterProfile>();
            CopyProxyRig copyProxyRig = character.GetComponent<CopyProxyRig>();

            if (characterProfile.characterDataSO.characterRoleSelect != CharacterData.CharacterRole.Player)
            {
                character.SetActive(false);
                copyProxyRig.enabled = false;
            }
            else 
            {
                character.SetActive(true);
                copyProxyRig.enabled = true;
            }
        }
    }
    void Update()
    {
        if (playerControls.GenericInput.YButton.triggered) 
        {
            yButtonEndSceneEvent.Invoke();
            //sceneSwitcher.SwitchToNextLevelKey();
            //sceneSwitcher.LoadNextScene();
        }
    }
}
