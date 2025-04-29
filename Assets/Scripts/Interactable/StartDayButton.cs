using TMPro;
using Unity.Netcode;
using UnityEngine;

public class StartDayButton : NetworkBehaviour, IInteractable
{
    public void OnInteract(PlayerInteractionManager interactionManager)
    {
        if (GameStateManager.Instance.CurrentDayState.Value == DayState.Preperation)
        {
            GameStateManager.Instance.BeginDay_Rpc();
        }
        else if (GameStateManager.Instance.CurrentDayState.Value == DayState.Closed)
        {
            GameStateManager.Instance.EndDay_Rpc();
        }
    }
}
