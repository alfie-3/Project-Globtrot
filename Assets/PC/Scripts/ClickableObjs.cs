using UnityEngine;

public class ClickableObjs : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isPowerButton = false;
    [SerializeField] private bool isScreen = false;
    [SerializeField] private PCHandler zoomScript;
    
    private bool on = false;

    public void OnInteract(PlayerInteractionManager interactionManager)
    {
        Debug.Log("Interacted");
        if (isScreen)
        {
            Debug.Log("Opening Screen");
            zoomScript.ZoomToScreen();
        }

        if (isPowerButton)
        {
            Debug.Log("Switching button");
            Material buttonMat = GetComponent<Renderer>().material;
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
