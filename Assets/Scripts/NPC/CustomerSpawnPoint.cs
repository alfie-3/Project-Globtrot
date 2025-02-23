using Unity.Netcode;
using UnityEngine;

public class CustomerSpawnPoint : NetworkBehaviour
{
    public static int customersCount;
    public int maxCustomers = 50;
    [Space]
    public float customerSpawnRate = 3;
    public bool spawningEnabled = false;

    [SerializeField] GameObject customerPrefab;

    [RuntimeInitializeOnLoadMethod]
    public static void Initialize()
    {
        customersCount = 0;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer) return;

        InvokeRepeating(nameof(SpawnCustomer), 0, customerSpawnRate);
    }

    public void SpawnCustomer()
    {
        if (customersCount >= maxCustomers) return;
        if (!spawningEnabled) return;

        NetworkObject customer = Instantiate(customerPrefab, transform.position, transform.rotation).GetComponent<NetworkObject>();
        customer.Spawn();
        customersCount++;
    }
}
