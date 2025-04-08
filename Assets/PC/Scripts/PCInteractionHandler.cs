using UnityEngine;

public class PCInteractionHandler : MonoBehaviour, IInteractable
{
    [SerializeField] private PCHandler pcScript;
    

    public void OnInteract(PlayerInteractionManager interactionManager)
    {
        pcScript.ZoomToScreen(interactionManager);
    }
}
