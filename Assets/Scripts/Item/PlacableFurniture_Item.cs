using Unity.AppUI.UI;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "New Furnture Item", menuName = "Items/Furniture Item")]
public class PlacableFurniture_Item : ItemBase
{
    [field: SerializeField] public GameObject FurniturePrefab { get; private set; }
    [field: SerializeField] public int FurniturePrice = 0;
    [field: SerializeField, TextArea(3,8)] public string Description;
    [field: SerializeField] public bool Required = false;

}
