using UnityEngine;
using UnityEngine.UI;

public class DebugGameManager : MonoBehaviour
{
    [SerializeField] private GameObject screen;
    [SerializeField] private GameObject zapper;
    [SerializeField] private GameObject debugSliderObject;

    [SerializeField] private BugSpawner bugSpawnerScript;
    [SerializeField] private Zapper zapperScript;

    [SerializeField] private AudioSource enterSceneAudio;
    [SerializeField] private AudioSource ambience;

    private Slider slider;
    private int bugsRemain;

    private DialogueManager dialogueManager;

    private CharacterManager characterManager;
    private CharacterData debuggedCharacterData;

    private void Start()
    {
        characterManager = FindObjectOfType<CharacterManager>();

        for (int i = 0; i < characterManager.characters.Length; i++)
        {
            GameObject character = characterManager.characters[i];
            CharacterData characterData = characterManager.characterDataSOs[i];
            character.SetActive(false);

            if (characterData.infectionLevel > 0 && !characterData.wasDebuggedLastRound)
            {
                character.SetActive(true);
                characterData.wasDebuggedLastRound = true;
                debuggedCharacterData = characterManager.characterDataSOs[i];
            }
        }

        dialogueManager = FindObjectOfType<DialogueManager>();

        bugSpawnerScript = FindObjectOfType<BugSpawner>();
        zapperScript = FindObjectOfType<Zapper>();
        slider = debugSliderObject.GetComponent<Slider>();

        screen.SetActive(false);
        zapper.SetActive(false);
        debugSliderObject.SetActive(false);

        bugsRemain = bugSpawnerScript.GetBugCount();
    }
    void Update()
    {
        int bugsZapped = zapperScript.GetBugZappedAmount();

        float debuggedFactor = 1- ( (float)bugsZapped / bugsRemain );
        slider.value = debuggedFactor;

        debuggedCharacterData.infectionLevel *= debuggedFactor; // DEBGUGGING THE CHARACTER

        bool endGame = bugSpawnerScript.DebugHasEnded();
        if (endGame) EndDebugGame();
    }

    public void StartDebugGame() 
    {
        screen.SetActive(true);
        zapper.SetActive(true);
        debugSliderObject.SetActive(true);
        bugSpawnerScript.StartCoroutine(bugSpawnerScript.SpawnBugsSequentially());
        enterSceneAudio.Play();
        ambience.Play();
    }

    public void EndDebugGame() 
    {
        screen.SetActive(false);
        zapper.SetActive(false);
        debugSliderObject.SetActive(false);
        dialogueManager.switchSceneEvent.Invoke();
        ambience.Stop();
    }
}
