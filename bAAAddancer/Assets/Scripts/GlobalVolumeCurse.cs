using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlobalVolumeCurse : MonoBehaviour
{
    private ClockCounter clockCounter;
    [SerializeField ] private VolumeProfile volumeProfile;
    [SerializeField] private ColorAdjustments colorAdjustments;

    void Start()
    {
        clockCounter = FindObjectOfType<ClockCounter>();

        volumeProfile.TryGet(out colorAdjustments);

        colorAdjustments.hueShift.value = 0f;
    }
    public void CurseGlobalVolume() 
    {
        StartCoroutine(CursedVolumeRoutine());
    }
    private IEnumerator CursedVolumeRoutine() 
    {
        float qBeatDuration = clockCounter.Get_Q_BeatInterval();

        colorAdjustments.hueShift.value = -138f;
        yield return new WaitForSeconds(qBeatDuration);

        colorAdjustments.hueShift.value = 47f;
        yield return new WaitForSeconds(qBeatDuration);

        colorAdjustments.hueShift.value = -138f;
        yield return new WaitForSeconds(qBeatDuration);

        colorAdjustments.hueShift.value = 47f;
        yield return new WaitForSeconds(qBeatDuration);
    }

    private void OnDestroy()
    {
        colorAdjustments.hueShift.value = 0f;
    }

}
