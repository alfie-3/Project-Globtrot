using TMPro;
using Unity.Multiplayer.Center.NetcodeForGameObjectsExample.DistributedAuthority;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PricegunHandler : NetworkBehaviour, IUsePrimary, IUseSecondary, IUpdate, IScroll
{
    StockShelfController currentShelfController;

    [SerializeField] TMP_Text priceReadoutText;
    double currentPrice;

    public void OnUpdate(PlayerHoldingManager manager)
    {
        Ray ray = new(manager.CameraManager.CamTransform.position, manager.CameraManager.CamTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 1.5f, LayerMask.GetMask("ItemShelf")))
        {
            if (hit.collider.TryGetComponent(out StockShelfController shelfController))
            {
                if (currentShelfController != shelfController)
                {
                    currentShelfController = shelfController;
                    UpdatePricing();
                    return;
                }
            }
        }
        else
        {
            currentShelfController = null;
            ClearPricing();
        }
    }

    public void UsePrimary(PlayerHoldingManager manager)
    {
        if (currentShelfController == null) return;

        currentShelfController.SetItemPricing(currentPrice);
    }

    public void UpdatePricing()
    {
        if (currentShelfController.Holder.IsEmpty) return;

        Stock_Item shopItem = (Stock_Item)ItemDictionaryManager.RetrieveItem(currentShelfController.Holder.ItemId.Value.ToString());
        if (shopItem == null) return;

        currentPrice = shopItem.GetCurrentSellPrice();
        priceReadoutText.text = MoneyFormatter.FormatPriceInt(currentPrice);
    }

    public void ClearPricing()
    {
        priceReadoutText.text = "-";
        currentPrice = 0;
    }

    public void OnScroll(PlayerHoldingManager manager, InputAction.CallbackContext context)
    {
        if (currentShelfController == null) return;

        float dir = context.ReadValue<float>();

        if (dir > 0)
        {
            currentPrice += 0.10;
        }
        else
        {
            currentPrice -= 0.10;
        }

        priceReadoutText.text = MoneyFormatter.FormatPriceInt(currentPrice);
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
