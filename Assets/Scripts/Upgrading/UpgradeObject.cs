using Unity.Netcode;
using UnityEngine;

public class UpgradeObject : NetworkBehaviour
{
    [SerializeField] bool DisableOnSpawn = true;

    public void Start()
    {
        if (DisableOnSpawn)
        {
            gameObject.SetActive(false);
        }
    }
}
