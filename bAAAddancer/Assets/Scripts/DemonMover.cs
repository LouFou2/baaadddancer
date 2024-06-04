using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;

public class DemonMover : MonoBehaviour
{
    [SerializeField] private DemonControllerParams demonHead;
    [SerializeField] private DemonControllerParams handL;
    [SerializeField] private DemonControllerParams handR;
    [SerializeField] private DemonControllerParams legL;
    [SerializeField] private DemonControllerParams legR;

    [Serializable]
    public class DemonControllerParams 
    {
        public GameObject demControlObject;
        public float xRange;
        public float yRange;
        public float zRange;
    }

    private ClockCounter clock;
    private float countDuration;

    float demHeadX;
    float demHeadY;
    float demHeadZ;

    float demHandLX;
    float demHandLY;
    float demHandLZ;

    float demHandRX;
    float demHandRY;
    float demHandRZ;

    float demLegLX;
    float demLegLY;
    float demLegLZ;

    float demLegRX;
    float demLegRY;
    float demLegRZ;

    float time = 0;

    void Start()
    {
        clock = FindObjectOfType<ClockCounter>();
        countDuration = clock.Get_Q_BeatInterval();
        Debug.Log(countDuration);

        demHeadX = demonHead.demControlObject.transform.position.x;
        demHeadY = demonHead.demControlObject.transform.position.y;
        demHeadZ = demonHead.demControlObject.transform.position.z;

        demHandLX = handL.demControlObject.transform.position.x;
        demHandLY = handL.demControlObject.transform.position.y;
        demHandLZ = handL.demControlObject.transform.position.z;

        demHandRX = handR.demControlObject.transform.position.x;
        demHandRY = handR.demControlObject.transform.position.y;
        demHandRZ = handR.demControlObject.transform.position.z;

        demLegLX = legL.demControlObject.transform.position.x;
        demLegLY = legL.demControlObject.transform.position.y;
        demLegLZ = legL.demControlObject.transform.position.z;

        demLegRX = legR.demControlObject.transform.position.x;
        demLegRY = legR.demControlObject.transform.position.y;
        demLegRZ = legR.demControlObject.transform.position.z;
    }

    void Update()
    {
        time += Time.deltaTime;
        float sinValue = Mathf.Sin((time / (countDuration*4)) * 2 * Mathf.PI);
        float sinTimes2 = Mathf.Sin((time / (countDuration * 8)) * 2 * Mathf.PI);
        float sinTimes3 = Mathf.Sin((time / (countDuration * 12)) * 2 * Mathf.PI);
        float sinTimes4 = Mathf.Sin((time / (countDuration * 16)) * 2 * Mathf.PI);

        float newDemHeadX = demHeadX + (sinTimes4 * demonHead.xRange);
        float newDemHeadY = demHeadY + (sinTimes4 * demonHead.yRange);
        float newDemHeadZ = demHeadZ + (sinTimes4 * demonHead.zRange);

        float newDemHandLX = demHandLX + (sinValue * handL.xRange);
        float newDemHandLY = demHandLY + (sinValue * handL.yRange);
        float newDemHandLZ = demHandLZ + (sinValue * handL.zRange);

        float newDemHandRX = demHandRX + (-sinValue * handR.xRange);
        float newDemHandRY = demHandRY + (-sinValue * handR.yRange);
        float newDemHandRZ = demHandRZ + (-sinValue * handR.zRange);

        float newDemLegLX = demLegLX + (sinTimes2 * handR.xRange);
        float newDemLegLY = demLegLY + (Mathf.Clamp(sinTimes2, 0, 1) * handR.yRange); // feet cant go through floor
        float newDemLegLZ = demLegLZ + (sinTimes2 * handR.zRange);

        float newDemLegRX = demLegRX + (-sinTimes2 * handR.xRange);
        float newDemLegRY = demLegRY + (Mathf.Clamp(-sinTimes2, 0, 1) * handR.yRange);
        float newDemLegRZ = demLegRZ + (-sinTimes2 * handR.zRange);

        demonHead.demControlObject.transform.position = new Vector3(newDemHeadX, newDemHeadY, newDemHeadZ);
        handL.demControlObject.transform.position = new Vector3(newDemHandLX, newDemHandLY, newDemHandLZ);
        handR.demControlObject.transform.position = new Vector3(newDemHandRX, newDemHandRY, newDemHandRZ);
        legL.demControlObject.transform.position = new Vector3(newDemLegLX, newDemLegLY, newDemLegLZ);
        legR.demControlObject.transform.position = new Vector3(newDemLegRX, newDemLegRY, newDemLegRZ);
    }
}
