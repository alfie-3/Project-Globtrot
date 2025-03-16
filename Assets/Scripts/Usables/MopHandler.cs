using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.WSA;

public class MopHandler : NetworkBehaviour, IUsePrimary, IUseSecondary, IOnHeld
{
    
    [SerializeField] float sweepDistance = 5f;
    [SerializeField] int sweepPower = 1;
    [SerializeField] int capacity = 4;
    [field: SerializeField] public NetworkVariable<int> fullness { get; private set; } = new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

    [SerializeField] TextMeshPro TextIndicator;
    //int fullness = 0;

    private void Start() {
        fullness.OnValueChanged += (previousValue, newValue) => TextIndicator.text = $"{newValue}/{capacity}";
    }


    public void OnHeld(PlayerHoldingManager _)
    {
        Debug.Log("Mop Equiped");
    }

    public void UsePrimary(PlayerHoldingManager holdingManager)
    {
        if(fullness.Value < capacity) Sweep(holdingManager);
    }

    public void UseSecondary(PlayerHoldingManager holdingManager) 
    {
        Ray ray = new(holdingManager.CameraManager.CamTransform.position, holdingManager.CameraManager.CamTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, sweepDistance)) {
            if (hit.collider.TryGetComponent(out Dumpster dumpster)) {
                dumpster.AddTrash();
                fullness.Reset();
                //fullness.Value = 0;
            }
        }
    }

    public void Sweep(PlayerHoldingManager holdingManager)
    {
        Ray ray = new(holdingManager.CameraManager.CamTransform.position, holdingManager.CameraManager.CamTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, sweepDistance))
        {
            if (hit.collider.TryGetComponent(out MessController messController))
            {
                messController.RequestSweep_Rpc(sweepPower);

                if (messController.Cleaned) {
                    Debug.Log("Cleened");
                    fullness.Value++;
                } else {
                    Debug.Log("Cleaning");
                }
            }
        }
    }
}
