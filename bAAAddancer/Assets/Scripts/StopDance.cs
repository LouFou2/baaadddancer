using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StopDance : MonoBehaviour
{
    [SerializeField] private CharacterManager characterManager;
    [SerializeField] private SceneSwitcher sceneSwitcher;
    [SerializeField] private AudioSource music;
    [SerializeField] private float pitchDecreaseSpeed = 0.1f;
    [SerializeField] private float yieldDuration = 0.1f;

    public UnityEvent StopDanceEvent;
    private void Start()
    {
        characterManager = FindObjectOfType<CharacterManager>();
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
        SwitchScene();
    }
    void SwitchScene() 
    {
        int currentRound = GameManager.Instance.GetCurrentRound();
        StopDanceEvent.Invoke();

        //sceneSwitcher.SwitchToNextLevelKey();
        //sceneSwitcher.LoadNextScene();
    }

}
