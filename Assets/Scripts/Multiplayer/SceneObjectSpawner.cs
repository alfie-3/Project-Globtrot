using Unity.Netcode;
using UnityEngine;

public class SceneObjectSpawner : NetworkBehaviour
{
    [SerializeField] NetworkObject prefab;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (prefab == null) return;

        if (IsServer)
        {
            NetworkObject nwO = Instantiate(prefab, transform.position, transform.rotation);
            nwO.Spawn();
        }
    }
}
