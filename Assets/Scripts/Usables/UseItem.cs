using UnityEngine;

[RequireComponent(typeof(StockItem))]
public class UseItem : MonoBehaviour, IUsePrimary
{
    StockItem stockItem;

    private void Awake()
    {
        stockItem = GetComponent<StockItem>();
    }

    public void UsePrimary(PlayerHoldingManager manager)
    {
        var mask = ~0;

        if (manager.UsageRaycast(mask, out RaycastHit hit))
        {
            if (hit.transform.TryGetComponent(out IUseItem useItem))
            {
                useItem.OnItemUsed(manager, stockItem.Item);
            }
        }
    }
}
