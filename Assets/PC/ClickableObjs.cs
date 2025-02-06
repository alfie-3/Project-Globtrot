using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class ClickableObjs : MonoBehaviour
{
    //[SerializeField] private bool isPowerButton = false;
    [SerializeField] private bool isScreen = false;
    [SerializeField] PCHandler zoomScript;
    public void OnClick()
    {
        if(isScreen)
        {
            zoomScript.ZoomToScreen();
        }
    }

}
