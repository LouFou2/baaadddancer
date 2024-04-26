using UnityEngine;

public class TitleFadeIn : MonoBehaviour
{
    [SerializeField] AudioSource titleAudioSource;
    public void TriggerTitleAudio()
    {
        //StartCoroutine(FadeTitle());
        titleAudioSource.Play();
    }
    
}
