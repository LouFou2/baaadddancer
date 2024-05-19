using UnityEngine;

public class MakeDanceSceneSetup : MonoBehaviour
{
    private CharacterManager characterManager;
    private PlayerControls playerControls;
    private SceneSwitcher sceneSwitcher;

    private ClockCounter clock;

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
        sceneSwitcher = FindObjectOfType<SceneSwitcher>();
        characterManager = FindObjectOfType<CharacterManager>();
        clock = FindObjectOfType<ClockCounter>();

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
        clock.StartClock();
    }
    void Update()
    {
        if (playerControls.GenericInput.YButton.triggered) 
        {
            sceneSwitcher.SwitchToNextLevelKey();
            sceneSwitcher.LoadNextScene();
        }
    }
}
