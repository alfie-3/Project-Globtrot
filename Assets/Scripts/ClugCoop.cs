using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class ClugCoop : NetworkBehaviour
{
    [SerializeField] NetworkObject clugPrefab;
    NetworkObject currentClug;

    [SerializeField] Transform clugSpawnTransform;

    [SerializeField] Upgrade ClugTrackerUpgrade;
    [SerializeField] Transform clugTracker;


    private void OnEnable()
    {
        GameStateManager.OnResetServer += ResetClug;
    }

    public override void OnNetworkSpawn()
    {
        SpawnClug();
        if(UpgradesManager.Instance.CurrentUpgrades.Contains(ClugTrackerUpgrade))
            StartTrackingClug();
        else
            UpgradesManager.OnUnlockedUpgrade += onUpgradeUnlocked;
    }

    private void OnClugDespawned()
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

    private void SpawnClug()
    {
        if (!IsServer) return;
        if (currentClug != null) return;
        NavMesh.SamplePosition(clugSpawnTransform.position, out NavMeshHit navHit, 246132, NavMesh.GetAreaFromName("Everything"));
        

        NetworkObject newClug = Instantiate(clugPrefab, navHit.position, clugSpawnTransform.rotation);
        newClug.Spawn();

        currentClug = newClug;
        newClug.GetComponent<Pickup_Interactable>().OnDespawned += OnClugDespawned;
    }

    private void onUpgradeUnlocked(Upgrade upgrade)
    {
        if (upgrade == ClugTrackerUpgrade)
        {
            StartTrackingClug();
            UpgradesManager.OnUnlockedUpgrade -= onUpgradeUnlocked;
        }
    }

    private void StartTrackingClug()
    {
        clugTracker.gameObject.SetActive(true);
        InvokeRepeating("trackClug", Random.value * 3, 2f + Random.value);
        
    }

    private void trackClug()
    {
        Vector3 lookDir = currentClug.transform.position - transform.position;
        lookDir.y = 0;
        Quaternion lookAtRotation = Quaternion.LookRotation(lookDir);
        clugTracker.DORotateQuaternion(lookAtRotation, 1.8f).SetEase(Ease.InOutExpo);
    }


    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (!IsServer) return;

        if (currentClug)
            currentClug.GetComponent<Pickup_Interactable>().OnDespawned -= OnClugDespawned;

        currentClug.Despawn();
    }

    private new void OnDestroy()
    {
        GameStateManager.OnResetServer -= ResetClug;
        UpgradesManager.OnUnlockedUpgrade -= onUpgradeUnlocked;
    }
}
