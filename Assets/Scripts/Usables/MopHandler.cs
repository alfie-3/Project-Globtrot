using Unity.Netcode;
using UnityEngine;

public class MopHandler : NetworkBehaviour, IUsePrimary, IOnHeld
{
    [SerializeField] float sweepDistance = 5f;
    [SerializeField] int sweepPower = 1;

    public void OnHeld(PlayerHoldingManager _)
    {
        Debug.Log("Mop Equiped");
    }

    public void UsePrimary(PlayerHoldingManager holdingManager)
    {
        Sweep(holdingManager);
    }

    public void Sweep(PlayerHoldingManager holdingManager)
    {
        Ray ray = new(holdingManager.CameraManager.CamTransform.position, holdingManager.CameraManager.CamTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, sweepDistance))
        {
            if (hit.collider.TryGetComponent(out MessController messController))
            {
                messController.RequestSweep_Rpc(sweepPower);
            }

        }
    }
}
