using Unity.Netcode;
using UnityEngine;

public class UpgradeObject : NetworkBehaviour
{
    [SerializeField] bool DisableOnSpawn = true;


    public override void OnNetworkSpawn()
    {
        if (DisableOnSpawn)
        {
            gameObject.SetActive(false);
        }
    }
}
