using TMPro;
using Unity.Netcode;
using UnityEngine;

public class StartDayButton : NetworkBehaviour, IInteractable
{
    public void OnInteract(PlayerInteractionManager interactionManager)
    {
        if (!GameStateManager.Instance.IsShopOpen)
        {
            GameStateManager.Instance.BeginDay_Rpc();
        }
        else
        {
            GameStateManager.Instance.ResetState_Rpc();
        }
    }
}
