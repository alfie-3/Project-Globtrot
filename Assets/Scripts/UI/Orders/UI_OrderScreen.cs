using UnityEngine;

public class UI_OrderScreen : MonoBehaviour
{
    [SerializeField] UI_OrderListItem orderListItemPrefab;
    [Space]
    [SerializeField] Transform orderListParent;

    public void AddOrder(Order order)
    {
        order.OnOrderRemoved += ClearOrder;

        foreach (Transform child in orderListParent)
        {
            ClearList();
        }

        foreach (OrderItem item in order.OrderItems)
        {
            UI_OrderListItem orderListItemUI = Instantiate(orderListItemPrefab, orderListParent);
            orderListItemUI.InitializeItem(item);
        }
    }

    public void ClearOrder(Order order)
    {
        order.OnOrderRemoved -= ClearOrder;

        ClearList();
    }

    public void ClearList()
    {
        foreach (Transform child in orderListParent)
        {
            Destroy(child.gameObject);
        }
    }
}
