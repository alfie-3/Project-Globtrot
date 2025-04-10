using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemDisplay : MonoBehaviour
{
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemQuantity;

    public void InitializeItem(ItemBase item, int quantity)
    {
        itemIcon.sprite = item.ItemIcon;
        itemName.text = item.ItemName;

        itemQuantity.text = quantity.ToString();
    }

    public void InitializeItem(OrderItem item)
    {
        itemIcon.sprite = item.Item.ItemIcon;
        itemName.text = item.Item.ItemName;

        itemQuantity.text = item.Quantity.ToString();
    }
}
