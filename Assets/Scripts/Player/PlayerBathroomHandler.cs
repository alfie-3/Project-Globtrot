using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI;

public class PlayerBathroomHandler : NetworkBehaviour
{
    [field: SerializeField] bool bathroomEnabled = false;
    [field: SerializeField] float BathroomNeed;
    [field: SerializeField] float MaxBathroomNeed = 120;
    [Space]
    [field: SerializeField] float sprintingBathroomMultiplier = 2;
    bool sprinting = false;
    [Space]
    public float BathroomNeedMultiplier = 1;
    PlayerCharacterController playerController;
    [Space]
    [SerializeField] MessController bathroomMessPrefab;

    public Action<float> OnBathroomNeedChange;

    public float NormalizedBathroom => BathroomNeed / MaxBathroomNeed;

    private void Awake()
    {
        BathroomNeed = 0.01f;

        if (TryGetComponent(out playerController))
        {
            playerController.OnSprintingChanged += ChangeSprintingMultiplier;
        }

        GameStateManager.OnDayStateChanged += UpdateBathroomEnabled;
        GameStateManager.OnReset += ResetBathroom;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsLocalPlayer)
            enabled = true;
    }

    private void ResetBathroom()
    {
        BathroomNeed = 0.01f;
        OnBathroomNeedChange?.Invoke(NormalizedBathroom);
    }

    private void UpdateBathroomEnabled(DayState state)
    {
        bathroomEnabled = state == DayState.Open || state == DayState.Overtime;
    }

    private void Update()
    {
        UpdateBathroom();

    }

    public void UpdateBathroom()
    {
        if (!bathroomEnabled) return;

        BathroomNeed += Time.deltaTime * BathroomNeedMultiplier * (sprinting == true ? sprintingBathroomMultiplier : 1);

        if (BathroomNeed >= MaxBathroomNeed)
        {
            Bathroom_Rpc();
            BathroomNeed = 0.01f;
        }

        OnBathroomNeedChange?.Invoke(NormalizedBathroom);
    }

    private void ChangeSprintingMultiplier(bool value)
    {
        sprinting = value;
    }

    public void Relieve()
    {
        BathroomNeed = 0;
        OnBathroomNeedChange?.Invoke(NormalizedBathroom);
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

    private new void OnDestroy()
    {
        base.OnDestroy();

        GameStateManager.OnDayStateChanged -= UpdateBathroomEnabled;
        GameStateManager.OnReset -= ResetBathroom;
    }
}
