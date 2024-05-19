using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource music;
    void Start()
    {
        
    }

    public void playMusic() 
    {
        music.Stop();
        music.Play();
    }
}
