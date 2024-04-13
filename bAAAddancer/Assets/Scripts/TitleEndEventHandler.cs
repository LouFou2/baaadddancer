using UnityEngine;

public class TitleEndEventHandler : MonoBehaviour
{
    public bool isTitleFinished = false;
    private void OnTitleAnimFinished() 
    {
        isTitleFinished = true;
    }
}
