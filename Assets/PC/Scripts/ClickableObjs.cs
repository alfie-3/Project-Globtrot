using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem.LowLevel;

public class ClickableObjs : MonoBehaviour
{
    [SerializeField] private bool isPowerButton = false;
    [SerializeField] private bool isScreen = false;
    [SerializeField] private PCHandler pcScript;
    
    private bool on = false;

    public void OnInteract(PlayerInteractionManager interactionManager)
    {
        if (isScreen)
        {
            pcScript.ZoomToScreen();
        }

        if(isPowerButton)
        {
            Material buttonMat = gameObject.GetOrAddComponent<Renderer>().material;
            if (on)
            {
                buttonMat.SetColor("_BaseColor", Color.red);
                pcScript.TurnOff();
                on = false;
            }
            else
            {
                buttonMat.SetColor("_BaseColor", Color.green);
                pcScript.TurnOn();
                on = true;
            }
        }
    }
}
