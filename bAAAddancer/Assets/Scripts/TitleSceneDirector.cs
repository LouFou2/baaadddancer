using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneDirector : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI chooseCharacterText;
    [SerializeField] private Image leftBumperImage;
    [SerializeField] private Image rightBumperImage;
    [SerializeField] private Image selectButtonImage;
    [SerializeField] private CharacterManager characterManager;
    [SerializeField] private GameObject[] charactersSelection;
    [SerializeField] private GameObject titleLights;
    [SerializeField] private GameObject characterLights;
    private int currentCharacterIndex = 0;
    private GameObject currentCharacter;
    [SerializeField] private TitleEndEventHandler titleEnd;
    private enum TitleSceneState { TitlePlaying, ChoosingCharacter, SceneEnd }
    private TitleSceneState titleSceneState;

    private bool bugIsAssigned = false;

    private PlayerControls playerControls;
    private SceneSwitcher sceneSwitcher;
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

        chooseCharacterText.text = string.Empty;
        leftBumperImage.gameObject.SetActive(false);
        rightBumperImage.gameObject.SetActive(false);
        selectButtonImage.gameObject.SetActive(false);

        characterLights.SetActive(false);
        titleLights.SetActive(true);

        charactersSelection = new GameObject[characterManager.characters.Length];

        bugIsAssigned = false;

        for (int i = 0; i < charactersSelection.Length; i++)
        {
            charactersSelection[i] = characterManager.characters[i];
            characterManager.characterDataSOs[i].characterRoleSelect = CharacterData.CharacterRole.NPC; //this just resets all characters to NPC's
            characterManager.characterDataSOs[i].infectionLevel = 0f;
            characterManager.characterDataSOs[i].wasDebuggedLastRound = false;
        }
        currentCharacter = charactersSelection[0];
        
        titleSceneState = TitleSceneState.TitlePlaying;
    }

    void Update()
    {
        switch (titleSceneState) 
        {
            case TitleSceneState.TitlePlaying:
                if (titleEnd != null && titleEnd.isTitleFinished)
                {
                    titleLights.SetActive(false);
                    titleSceneState = TitleSceneState.ChoosingCharacter;
                }
                break;

            case TitleSceneState.ChoosingCharacter:
                characterLights.SetActive(true);
                chooseCharacterText.text = "select character";
                leftBumperImage.gameObject.SetActive(true);
                rightBumperImage.gameObject.SetActive(true);
                selectButtonImage.gameObject.SetActive(true);

                HandleCharacterSelection();

                break;

            case TitleSceneState.SceneEnd:
                chooseCharacterText.text = string.Empty;
                leftBumperImage.gameObject.SetActive(false);
                rightBumperImage.gameObject.SetActive(false);
                selectButtonImage.gameObject.SetActive(false);

                AssignBugCharacter();
                HandleSceneEnd();

                break;
        }
    }

    void HandleCharacterSelection() 
    {
        if (playerControls.DanceControls.SwitchObjectL.triggered) 
        {
            currentCharacterIndex --;
        }
        if (playerControls.DanceControls.SwitchObjectR.triggered)
        {
            currentCharacterIndex ++;
        }
        if (currentCharacterIndex > charactersSelection.Length - 1) 
        {
            currentCharacterIndex = 0;
        }
        if (currentCharacterIndex < 0) 
        {
            currentCharacterIndex = charactersSelection.Length - 1;
        }
        currentCharacter = charactersSelection[currentCharacterIndex];

        foreach (var character in charactersSelection) 
        {
            if (character != currentCharacter)
            {
                character.SetActive(false);
            }
            else 
            {
                character.SetActive(true);
                character.transform.position = Vector3.zero;
            }
        }
        if (playerControls.DanceControls.AButton.triggered) 
        {
            // this is probably a bad way to do this, but oh well:
            characterManager.characterDataSOs[currentCharacterIndex].characterRoleSelect = CharacterData.CharacterRole.Player;
            currentCharacter.SetActive(false);
            titleSceneState = TitleSceneState.SceneEnd;
        }

    }
    void AssignBugCharacter() 
    {
        if (!bugIsAssigned) 
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, charactersSelection.Length);
            }
            while (randomIndex == currentCharacterIndex); // Check if the random index is the same as the player character index

            characterManager.characterDataSOs[randomIndex].characterRoleSelect = CharacterData.CharacterRole.Bug;
            bugIsAssigned = true;
        }
        
    }
    void HandleSceneEnd() 
    {
        sceneSwitcher.SwitchToNextLevelKey();
        sceneSwitcher.LoadNextScene();
    }
}