using UnityEngine;

public class BathroomInteractable : MonoBehaviour, IInteractable, IViewable
{
    public void OnInteract(PlayerInteractionManager interactionManager)
    {
        if (interactionManager.TryGetComponent(out PlayerBathroomHandler bathroom))
        {
            if (bathroom.NormalizedBathroom < 0.33f)
            {
                UI_Notifcation.EnqueueNotification("YOU DONT NEED THIS YET");
                return;
            }

            bathroom.Relieve();
        }
    }

    public void OnUnview()
    {
        Debug.Log("Unview");
    }

    public InteractionContext OnView()
    {
        return new(true, "Relieve");
    }

}
