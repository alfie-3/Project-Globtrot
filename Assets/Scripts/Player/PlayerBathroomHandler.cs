using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerBathroomHandler : NetworkBehaviour
{
    [field: SerializeField] float BathroomNeed;
    [field: SerializeField] float MaxBathroomNeed = 120;
    [Space]
    [field: SerializeField] float sprintingBathroomMultiplier = 2;
    [Space]
    float bathroomNeedMultiplier = 1;
    PlayerCharacterController playerController;
    [Space]
    [SerializeField] MessController bathroomMessPrefab;

    public Action<float> OnBathroomNeedChange;

    public float NormalizedBathroom => BathroomNeed / MaxBathroomNeed;

    private void Awake()
    {
        BathroomNeed = 0;

        if (TryGetComponent(out playerController))
        {
            playerController.OnSprintingChanged += ChangeSprintingMultiplier;
        }
    }

    private void Update()
    {
        BathroomNeed += Time.deltaTime * bathroomNeedMultiplier;

        if (BathroomNeed >= MaxBathroomNeed)
        {
            Bathroom_Rpc();
            BathroomNeed = 0;
        }

        OnBathroomNeedChange.Invoke(NormalizedBathroom);

    }

    private void ChangeSprintingMultiplier(bool value)
    {
        bathroomNeedMultiplier = value == true ? sprintingBathroomMultiplier : 1;
    }

    public void Relieve()
    {
        BathroomNeed = 0;
    }

    [Rpc(SendTo.Everyone)]
    public void Bathroom_Rpc()
    {
        if (IsServer)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 5, LayerMask.GetMask("Default", "Placeable")))
            {
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 2, NavMesh.GetAreaFromName("Everything")))
                {
                    Vector3 euler = new(90, UnityEngine.Random.Range(0, 360), 0);
                    MessController instancedMessController = Instantiate(bathroomMessPrefab, navHit.position + bathroomMessPrefab.transform.position, Quaternion.Euler(euler));
                    instancedMessController.GetComponent<NetworkObject>().Spawn();
                }
            }
        }
    }
}
