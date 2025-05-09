using Unity.Netcode;
using UnityEngine;

public class SceneObjectDisabler : NetworkBehaviour
{
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        gameObject.SetActive(false);
    }
}
