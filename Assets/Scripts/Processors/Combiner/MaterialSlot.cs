using System.Collections.Generic;
using UnityEngine;

public class MaterialSlot : ItemSlot
{
    [SerializeField]
    List<Stock_Item> materials;
    public override bool CanUseItem(PlayerHoldingManager holdingManager, Stock_Item item)
    {
        if (materials.Contains(item)) return true;
        return false;
    }
}
