using UnityEngine;

public class StockItem : MonoBehaviour
{
    [field: SerializeField] public Stock_Item Item { get; private set; }

    public void SetItem(Stock_Item item)
    {
        Item = item;
    }
}
