using TMPro;
using Unity.Netcode;
using UnityEngine;

public class StartDayButton : NetworkBehaviour, IInteractable
{
    public void OnInteract(PlayerInteractionManager interactionManager)
    {
        if (!GameStateManager.Instance.IsOpen)
        {
            GameStateManager.Instance.BeginDay_Rpc();
        }
        else
        {
            GameStateManager.Instance.EndDay_Rpc();
        }
    }
}
