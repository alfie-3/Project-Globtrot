using UnityEngine;

public class Test_Interactable : MonoBehaviour, IInteractable, IViewable
{
    public void OnInteract(PlayerInteractionManager interactionManager)
    {
        Debug.Log("Interacted");
    }

    public void OnUnview()
    {
        Debug.Log("Unview");
    }

    public void OnView()
    {
        Debug.Log("Viewed");
    }
}
