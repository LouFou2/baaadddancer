using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundResetter : MonoBehaviour
{
    [SerializeField] private GameManagerData gameManagerData;

    private void OnDisable()
    {
        gameManagerData.currentRound = -1;
    }
}
