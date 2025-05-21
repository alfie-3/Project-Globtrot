using DG.Tweening;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class ClugCoop : NetworkBehaviour
{
    [SerializeField] NetworkObject clugPrefab;
    NetworkObject currentClug;

    [SerializeField] Transform clugSpawnTransform;

    [SerializeField] Transform clugTracker;

    private void OnEnable()
    {
        GameStateManager.OnResetServer += ResetClug;
    }

    public override void OnNetworkSpawn()
    {
        //Invoke("SpawnClug", 3f);
        SpawnClug();
        StartTrackingClug();
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

    public void SpawnClug()
    {
        if (!IsServer) return;
        if (currentClug != null) return;
        NavMesh.SamplePosition(clugSpawnTransform.position, out NavMeshHit navHit, 246132, NavMesh.GetAreaFromName("Everything"));
        

        NetworkObject newClug = Instantiate(clugPrefab, navHit.position, clugSpawnTransform.rotation);
        newClug.Spawn();

        currentClug = newClug;
        newClug.GetComponent<Pickup_Interactable>().OnDespawned += OnClugDespawned;
    }


    public void StartTrackingClug()
    {
        clugTracker.gameObject.SetActive(true);
        InvokeRepeating("trackClug", 1f, 2f);
        
    }

    private void trackClug()
    {
        Quaternion lookAtRotation = Quaternion.LookRotation(currentClug.transform.position - transform.position);
        //lookAtRotation = new Quaternion(0, lookAtRotation.y, 0, lookAtRotation.z);

        Vector3 lookDir = currentClug.transform.position - transform.position;
        //lookDir.z = 0;
        //clugTracker.transform.LookAt(currentClug.transform.position,Vector3.up);
        //clugTracker.transform.rotation = new Quaternion(0, clugTracker.transform.rotation.y, 0, clugTracker.transform.rotation.z);
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
    }
}
