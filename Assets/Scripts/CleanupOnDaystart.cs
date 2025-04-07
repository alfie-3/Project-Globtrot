using Unity.Netcode;
using UnityEngine;

public class CleanupOnDaystart : NetworkBehaviour
{
    private void OnEnable()
    {
        GameStateManager.OnResetServer += Despawn;
    }

    private void OnDisable()
    {
        GameStateManager.OnResetServer -= Despawn;
    }

    public void Despawn()
    {
        if (!IsServer) return;

        NetworkObject.Despawn();
    }

    private new void OnDestroy()
    {
        base.OnDestroy();
        GameStateManager.OnResetServer -= Despawn;
    }
}
