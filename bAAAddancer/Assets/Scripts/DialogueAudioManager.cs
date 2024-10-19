using UnityEngine;

public class DialogueAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource oneShot;

    [SerializeField] private CharVOXManager[] charVOXManagers; // add in inspector, in same order as Character Manager char data SO's

    public void HandleAudio(int speakerIndex, AudioClip oneShotClip, AudioClip newMusicTrack, VoxEmote voxEmote)
    {
        if (oneShotClip != null)
        {
            oneShot.clip = oneShotClip;
            oneShot.Play();
        }
        if (newMusicTrack != null)
        {
            music.Stop();
            music.clip = newMusicTrack;
            music.Play();
        }
        switch (voxEmote)
        {
            case VoxEmote.casual:
                charVOXManagers[speakerIndex].PlayCasualVox();
                break;
            case VoxEmote.confused:
                charVOXManagers[speakerIndex].PlayConfusedVox();
                break;
            case VoxEmote.happy:
                charVOXManagers[speakerIndex].PlayHappyVox();
                break;
            case VoxEmote.mad:
                charVOXManagers[speakerIndex].PlayMadVox();
                break;
            case VoxEmote.pleading:
                charVOXManagers[speakerIndex].PlayPleadingVox();
                break;
            case VoxEmote.worried:
                charVOXManagers[speakerIndex].PlayWorriedVox();
                break;
            default:
                charVOXManagers[speakerIndex].PlayCasualVox();
                break;
        }
        
    }
    
}
