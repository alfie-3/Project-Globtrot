using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Button_Interactable : NetworkBehaviour, IInteractable
{
    [SerializeField] UnityEvent PressedEvent;
    [SerializeField] bool replicate = true;

    public void OnInteract(PlayerInteractionManager interactionManager)
    {
        if (!replicate)
        {
            PressedEvent.Invoke();
        } else
        {
            Interact_Rpc();
        }
    }

    [Rpc(SendTo.Everyone)]
    public void Interact_Rpc()
    {
        PressedEvent.Invoke();
    }
}
