using UnityEngine;

public class ClickableObjs : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isPowerButton = false;
    [SerializeField] private bool isScreen = false;
    [SerializeField] private PCHandler pcScript;
    [SerializeField] private GameObject onIndicator;
    
    private bool on = false;

    public void OnInteract(PlayerInteractionManager interactionManager)
    {
        if (isScreen)
        {
            pcScript.ZoomToScreen(interactionManager);
        }

        Power();
    }

    public void Power()
    {
        if (isPowerButton)
        {
            Material buttonMat = onIndicator.GetComponent<Renderer>().material;
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
