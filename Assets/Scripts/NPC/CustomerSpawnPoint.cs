using Unity.Netcode;
using UnityEditor;
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

        InvokeRepeating(nameof(SpawnCustomersRepeating), 0, customerSpawnRate);
    }

    public void SpawnCustomersRepeating()
    {
        if (customersCount >= maxCustomers) return;
        if (!spawningEnabled) return;

        SpawnCustomer();
    }


    [ContextMenu("Spawn Customer")]
    public void SpawnCustomer()
    {
        NetworkObject customer = Instantiate(customerPrefab, transform.position, transform.rotation).GetComponent<NetworkObject>();
        customer.Spawn();
        customersCount++;
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(CustomerSpawnPoint))]
class CustomSpawnPointEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DrawDefaultInspector();

        CustomerSpawnPoint customerSpawner = (CustomerSpawnPoint)target;

        if (GUILayout.Button("SpawnCustomer"))
        {
            customerSpawner.SpawnCustomer();
        }
    }
}
#endif