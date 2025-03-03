using Unity.Netcode;
using UnityEngine;

public class MopHandler : NetworkBehaviour, IUsePrimary, IUseSecondary, IOnHeld
{
    [SerializeField] float sweepDistance = 5f;
    [SerializeField] int sweepPower = 1;
    [SerializeField] int capacity = 4;
    int fullness = 0;

    public void OnHeld(PlayerHoldingManager _)
    {
        Debug.Log("Mop Equiped");
    }

    public void UsePrimary(PlayerHoldingManager holdingManager)
    {
        if(fullness < capacity) Sweep(holdingManager);
    }

    public void UseSecondary(PlayerHoldingManager holdingManager) 
    {
        Ray ray = new(holdingManager.CameraManager.CamTransform.position, holdingManager.CameraManager.CamTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, sweepDistance)) {
            if (hit.collider.TryGetComponent(out Dumpster dumpster)) {
                dumpster.AddTrash();
                fullness = 0;
                
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
                    fullness++;
                } else {
                    Debug.Log("Cleaning");
                }
            }
        }
    }
}
