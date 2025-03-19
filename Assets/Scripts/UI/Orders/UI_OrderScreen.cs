using UnityEngine;

public class UI_OrderScreen : MonoBehaviour
{
    [SerializeField] UI_OrderListItem orderListItemPrefab;
    [Space]
    [SerializeField] Transform orderListParent;

    public void AddOrder(Order order)
    {
        order.OnOrderFailed += ClearOrder;
        order.OnOrderSucceeded += ClearOrder;

        foreach (Transform child in orderListParent)
        {
            Destroy(child.gameObject);
        }

        foreach (OrderItem item in order.orderItems)
        {
            UI_OrderListItem orderListItemUI = Instantiate(orderListItemPrefab, orderListParent);
            orderListItemUI.InitializeItem(item);
        }
    }

    public void ClearOrder(Order order)
    {
        order.OnOrderFailed -= ClearOrder;
        order.OnOrderSucceeded -= ClearOrder;
    }
}
