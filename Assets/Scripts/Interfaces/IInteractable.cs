using UnityEngine;

public interface IInteractable 
{
    public void OnView();
    public void OnUnview();

    public void OnInteract(PlayerInteractionManager interactionManager);
}
