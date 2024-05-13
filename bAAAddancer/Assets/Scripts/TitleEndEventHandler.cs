using UnityEngine;

public class TitleEndEventHandler : MonoBehaviour
{
    public bool isTitleFinished = false;

    public void Awake()
    {
        isTitleFinished = false;
    }

    public void OnTitleAnimFinished() 
    {
        isTitleFinished = true;
    }
}
