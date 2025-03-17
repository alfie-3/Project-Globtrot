using Unity.AppUI.UI;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "New Furnture Item", menuName = "Items/Furniture Item")]
public class PlacableFurniture_Item : ItemBase
{
    [field: SerializeField] public GameObject FurniturePrefab { get; private set; }
    [field: SerializeField] public float FurniturePrice = 0.0f;
    [field: SerializeField] public ProductCategory Category { get; private set; } 
    [field: SerializeField] public float UnlockPrice = 50.0f;
    [field: SerializeField] public bool Unlockable = false;

}
