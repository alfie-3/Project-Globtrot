using Unity.Netcode;
using UnityEngine;

public class PricegunHandler : NetworkBehaviour, IUsePrimary, IUseSecondary
{
    public void UsePrimary(PlayerHoldingManager manager)
    {
        Ray ray = new(manager.CameraManager.CamTransform.position, manager.CameraManager.CamTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 1.5f, LayerMask.GetMask("ItemShelf")))
        {
            if (hit.collider.TryGetComponent(out StockShelfController shelfController))
            {
                shelfController.SetItemPricing();
            }

        }
    }

    public void UseSecondary(PlayerHoldingManager manager)
    {
        Tase();
    }

    public void Tase()
    {
        Debug.Log("Zap");
    }
}
