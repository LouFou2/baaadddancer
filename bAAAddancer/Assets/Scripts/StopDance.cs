using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopDance : MonoBehaviour
{
    [SerializeField] private SceneSwitcher sceneSwitcher;
    [SerializeField] private AudioSource music;
    [SerializeField] private float pitchDecreaseSpeed = 0.1f;
    [SerializeField] private float yieldDuration = 0.1f;

    public void StopTheDance()
    {
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
        sceneSwitcher.SwitchToNextLevelKey();
        sceneSwitcher.LoadNextScene();
    }

}
