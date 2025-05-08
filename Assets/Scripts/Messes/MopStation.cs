using System;
using Unity.Netcode;
using UnityEngine;

public class MopStation : NetworkBehaviour
{
    [SerializeField] NetworkObject mopPrefab;
    NetworkObject currentMop;

    [SerializeField] Transform mopSpawnTransform;

    private void OnEnable()
    {
        GameStateManager.OnResetServer += ResetMop;
    }

    public override void OnNetworkSpawn()
    {
        SpawnMop();
    }

    private void OnMopDespawned()
    {
        currentMop = null;
        ResetMop();
    }

    private void ResetMop()
    {
        if (currentMop != null)
        {
            Rigidbody rigidbody = currentMop.GetComponent<Rigidbody>();
            rigidbody.MovePosition(mopSpawnTransform.position);
            rigidbody.MoveRotation(mopSpawnTransform.rotation);
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.linearVelocity = Vector3.zero;
        }
        else
        {
            SpawnMop();
        }
    }

    public void SpawnMop()
    {
        if (!IsServer) return;
        if (currentMop != null) return;

        NetworkObject newMop = Instantiate(mopPrefab, mopSpawnTransform.position, mopSpawnTransform.rotation);
        newMop.Spawn();

        currentMop = newMop;
        newMop.GetComponent<Pickup_Interactable>().OnDespawned += OnMopDespawned;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (!IsServer) return;

        if (currentMop)
            currentMop.GetComponent<Pickup_Interactable>().OnDespawned -= OnMopDespawned;

        currentMop.Despawn();
    }

    private new void OnDestroy()
    {
        GameStateManager.OnResetServer -= ResetMop;
    }
}
