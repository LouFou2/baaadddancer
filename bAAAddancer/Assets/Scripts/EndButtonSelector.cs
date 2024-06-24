using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndButtonSelector : MonoBehaviour
{
    public Button endButton; 

    public void SelectEndButton()
    {
        endButton.Select();
    }
}
