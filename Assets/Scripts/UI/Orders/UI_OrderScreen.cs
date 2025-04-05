using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_OrderScreen : MonoBehaviour
{
    [SerializeField] UI_OrderListItem orderListItemPrefab;
    [Space]
    [SerializeField] Transform orderListParent;
    [SerializeField] Image timerThrobberImage;
    [Space]
    [SerializeField] Canvas SuccessCanvas;
    [SerializeField] Canvas FailCanvas;

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

    public void PlayResponse(bool success)
    {
        StartCoroutine(PlayResponseRoutine(success));
    }

    public IEnumerator PlayResponseRoutine(bool success)
    {
        if (success)
        {
            SuccessCanvas.enabled = true;
        }
        else
        {
            FailCanvas.enabled = true;
        }

        yield return new WaitForSeconds(3);

        if (success)
        {
            SuccessCanvas.enabled = false;
        }
        else
        {
            FailCanvas.enabled = false;
        }
    }
}
