using UnityEngine;
using UnityEngine.UI;

public class UI_OrderScreen : MonoBehaviour
{
    [SerializeField] UI_OrderListItem orderListItemPrefab;
    [Space]
    [SerializeField] Transform orderListParent;
    [SerializeField] Image timerThrobberImage;

    public void AddOrder(Order order)
    {
        order.OnOrderRemoved += ClearOrder;
        order.OnOrderTimerUpdate += OnTimerUpdate;

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
        order.OnOrderTimerUpdate -= OnTimerUpdate;

        ClearList();
    }

    public void ClearList()
    {
        foreach (Transform child in orderListParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void OnTimerUpdate(float initalTime, float currentTime)
    {
        timerThrobberImage.fillAmount = currentTime / initalTime;
    }
}
