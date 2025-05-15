using System;
using Unity.Netcode;
using UnityEngine;

public class ClugCoop : NetworkBehaviour
{
    [SerializeField] NetworkObject clugPrefab;
    NetworkObject currentClug;

    [SerializeField] Transform clugSpawnTransform;

    private void OnEnable()
    {
        GameStateManager.OnResetServer += ResetClug;
    }

    public override void OnNetworkSpawn()
    {
        SpawnClug();
    }

    private void OnMopDespawned()
    {
        currentClug = null;
        ResetClug();
    }

    private void ResetClug()
    {
        if (currentClug != null)
        {
            Rigidbody rigidbody = currentClug.GetComponent<Rigidbody>();
            rigidbody.MovePosition(clugSpawnTransform.position);
            rigidbody.MoveRotation(clugSpawnTransform.rotation);
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.linearVelocity = Vector3.zero;
        }
        else
        {
            SpawnClug();
        }
    }

    public void SpawnClug()
    {
        if (!IsServer) return;
        if (currentClug != null) return;

        NetworkObject newMop = Instantiate(clugPrefab, clugSpawnTransform.position, clugSpawnTransform.rotation);
        newMop.Spawn();

        currentClug = newMop;
        newMop.GetComponent<Pickup_Interactable>().OnDespawned += OnMopDespawned;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (!IsServer) return;

        if (currentClug)
            currentClug.GetComponent<Pickup_Interactable>().OnDespawned -= OnMopDespawned;

        currentClug.Despawn();
    }

    private new void OnDestroy()
    {
        GameStateManager.OnResetServer -= ResetClug;
    }
}
