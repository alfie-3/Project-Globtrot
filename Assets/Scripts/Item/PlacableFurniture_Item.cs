using Unity.AppUI.UI;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "New Furnture Item", menuName = "Items/Furniture Item")]
public class PlacableFurniture_Item : ItemBase
{
    [field: SerializeField] public GameObject FurniturePrefab { get; private set; }
    [field: SerializeField] public int FurniturePrice = 0;
    [field: SerializeField] public string Description;
    [field: SerializeField] public ProductCategory Category { get; private set; } 
    [field: SerializeField] public int UnlockPrice = 50;
    [field: SerializeField] public bool Unlockable = false;
    [field: SerializeField] public float SalePercentage = 0.25f;

    // allows for discounts
    public int GetCurrentPurchasePrice()
    {
        return UnlockPrice;
    }

}
