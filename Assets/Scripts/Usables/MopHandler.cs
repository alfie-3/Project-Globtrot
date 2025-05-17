using TMPro;
using Unity.Netcode;
using UnityEngine;

public class MopHandler : NetworkBehaviour, IUsePrimary, IOnHeld, IOnDrop
{
    
    [SerializeField] float sweepDistance = 5f;
    [SerializeField] int sweepPower = 1;

    public void OnHeld(PlayerHoldingManager manager)
    {
        manager.GetComponentInChildren<Animator>().SetLayerWeight(1, 1);
        manager.GetComponentInChildren<Animator>().CrossFade("HoldMop", 0.1f);
    }

    public void OnDrop(PlayerHoldingManager manager)
    {
        manager.GetComponentInChildren<Animator>().Play("Unequip", 1);
        //manager.GetComponentInChildren<Animator>().SetLayerWeight(1, 0);
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

                if (messController.Cleaned) {
                    Debug.Log("Cleened");
                } else {
                    Debug.Log("Cleaning");
                }
            }
        }
    }

    public InteractionContext GetUseContext(PlayerHoldingManager holdingManager)
    {
        Ray ray = new(holdingManager.CameraManager.CamTransform.position, holdingManager.CameraManager.CamTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, sweepDistance))
        {
            if (hit.collider.TryGetComponent(out MessController messController))
            {
                return new(true, "Clean");
            }
        }

        return new(false);
    }
}
