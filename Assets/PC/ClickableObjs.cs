using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem.LowLevel;

public class ClickableObjs : MonoBehaviour
{
    [SerializeField] private bool isPowerButton = false;
    [SerializeField] private bool isScreen = false;
    [SerializeField] PCHandler zoomScript;
    bool on = false;
    public void OnClick()
    {
        if(isScreen)
        {
            zoomScript.ZoomToScreen();
        }

        if(isPowerButton)
        {
            Material buttonMat = gameObject.GetOrAddComponent<Renderer>().material;
            if (on)
            {
                buttonMat.SetColor("_BaseColor", Color.red);
                zoomScript.TurnOff();
                on = false;
            }
            else
            {
                buttonMat.SetColor("_BaseColor", Color.green);
                zoomScript.TurnOn();   
                on = true;
            }
        }
    }
}
