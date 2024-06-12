using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class MegaMixTrackSwitcher : MonoBehaviour
{
    [SerializeField] private RaveDemonRevealer raveDemonRevealerScript;

    [SerializeField] private AudioClip demonWinsTrack;
    [SerializeField] private AudioClip playerWinsTrack;

    [SerializeField] private PlayableDirector playableDirector;
    [SerializeField] private TimelineAsset timelineAsset;

    private bool playerWins = true;

    private void Start()
    {
        playerWins = raveDemonRevealerScript.CheckIfPlayerWins();

        if (timelineAsset != null)
        {
            TrackAsset audioTrack = null;

            // Find the single Audio Track in the Timeline
            foreach (var track in timelineAsset.GetOutputTracks())
            {
                if (track is AudioTrack)
                {
                    audioTrack = track;
                    break;
                }
            }

            if (audioTrack != null)
            {
                TimelineClip clip = null;

                // Get the first clip on the audio track
                foreach (var c in audioTrack.GetClips())
                {
                    clip = c;
                    break;
                }

                if (clip != null && clip.asset is AudioPlayableAsset audioPlayableAsset)
                {
                    // Replace the audio clip
                    audioPlayableAsset.clip = playerWins ? playerWinsTrack : demonWinsTrack;
                }
            }

            // Rebind the timeline asset to the playable director
            playableDirector.Play(timelineAsset);
        }
    }
}
