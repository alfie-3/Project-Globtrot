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

    private void OnEnable()
    {
        //GameStateManager.OnDayStateChanged += ToggleSpawning;
    }

    private void OnDisable()
    {
        //GameStateManager.OnDayStateChanged -= ToggleSpawning;
    }

    public void ToggleSpawning(bool toggle)
    {
        if (!IsServer) return;


        if (toggle)
        {
            spawningEnabled = true;
            InvokeRepeating(nameof(SpawnCustomersRepeating), 0, customerSpawnRate);
        }
        else
        {
            spawningEnabled = false;
            CancelInvoke(nameof(SpawnCustomersRepeating));
        }
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

    private new void OnDestroy()
    {
        base.OnDestroy();

        //GameStateManager.OnDayStateChanged -= ToggleSpawning;
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