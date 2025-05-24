using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StopDance : MonoBehaviour
{
    private PlayerControls playerControls;
    [SerializeField] private CharacterManager characterManager;
    [SerializeField] private SceneSwitcher sceneSwitcher;
    [SerializeField] private AudioSource music;
    [SerializeField] private float pitchDecreaseSpeed = 0.1f;
    [SerializeField] private float yieldDuration = 0.1f;

    private int roundIndex = -1;

    public UnityEvent StopDanceEvent;
    public UnityEvent GotoRaveEvent;

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

    private void Start()
    {
        characterManager = FindObjectOfType<CharacterManager>();

        roundIndex = GameManager.Instance.GetCurrentRound();
    }
    private void Update()
    {
        //only listening for "return" button
        if (playerControls.GenericInput.HomeButton.triggered)
        {
            Debug.Log("click!");
            EndRaveReturn();
        }
    }
    public void StopTheDance()
    {
        foreach (GameObject character in characterManager.characters) 
        {
            Animator animator = character.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("Dance", false);
            }
            else
            {
                Debug.LogWarning("Animator not found on character: " + character.name);
            }
        }
        StartCoroutine(EndDance());
    }

    private IEnumerator EndDance()
    {
        while (music.pitch > 0f)
        {
            music.pitch -= pitchDecreaseSpeed;
            music.volume -= pitchDecreaseSpeed;

            if (music.pitch < 0f)
            {
                music.pitch = 0f;
                music.volume = 0f;
            }
            yield return new WaitForSeconds(yieldDuration);
        }
        if (roundIndex < 3) // until round 4 we do the practice gameloop
            StopDanceEvent?.Invoke(); // Switches Scene + Round
        else if (roundIndex == 3) // end of round 4 we go to the rave
            GotoRaveEvent?.Invoke();
    }
    
    private void EndRaveReturn()
    {
        Debug.Log("click!");
        GameManager.Instance.RestartGame();
    }

}
