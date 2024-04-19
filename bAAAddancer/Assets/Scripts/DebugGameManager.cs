using UnityEngine;
using UnityEngine.UI;

public class DebugGameManager : MonoBehaviour
{
    [SerializeField] private GameObject screen;
    [SerializeField] private GameObject zapper;
    [SerializeField] private GameObject debugSliderObject;

    [SerializeField] private BugSpawner bugSpawnerScript;
    [SerializeField] private Zapper zapperScript;

    private Slider slider;
    private int bugsRemain;

    private DialogueManager dialogueManager;

    private void Start()
    {
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

        slider.value = 1- ( (float)bugsZapped / bugsRemain );
        bool endGame = bugSpawnerScript.DebugHasEnded();
        if (endGame) EndDebugGame();
    }

    public void StartDebugGame() 
    {
        screen.SetActive(true);
        zapper.SetActive(true);
        debugSliderObject.SetActive(true);
        bugSpawnerScript.StartCoroutine(bugSpawnerScript.SpawnBugsSequentially());
    }

    public void EndDebugGame() 
    {
        screen.SetActive(false);
        zapper.SetActive(false);
        debugSliderObject.SetActive(false);
        dialogueManager.switchSceneEvent.Invoke();
    }
}
