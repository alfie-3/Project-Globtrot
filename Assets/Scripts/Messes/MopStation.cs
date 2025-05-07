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
        if (currentMop == null)
        {
            SpawnMop();
        }
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
        NetworkObject newMop = Instantiate(mopPrefab, mopSpawnTransform.position, mopSpawnTransform.rotation);
        newMop.Spawn();

        currentMop = newMop;
    }

    private new void OnDestroy()
    {
        GameStateManager.OnResetServer -= ResetMop;
    }
}
