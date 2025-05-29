using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_OrderScreen : MonoBehaviour
{
    [SerializeField] UI_ItemDisplay orderListItemPrefab;
    [Space]
    [SerializeField] Transform orderListParent;
    [Space]
    [SerializeField] Image timerThrobberImage;
    [SerializeField] Sprite timerThrobberSprite;
    [SerializeField] Sprite timerWaitingSprite;
    [Space]
    [SerializeField] Canvas SuccessCanvas;
    [SerializeField] Canvas FailCanvas;
    [Space]
    [SerializeField] TextMeshProUGUI[] ScreenText;

    private void Awake()
    {
        GameStateManager.OnDayStateChanged += UpdateScreenText;
    }

    private void UpdateScreenText(DayState state)
    {
        foreach (TextMeshProUGUI tmpUI in ScreenText)
        {
            switch (state)
            {
                case (DayState.Preperation):
                    tmpUI.enabled = true;
                    tmpUI.text = "CLOCK IN TO START DAY";
                    break;

                case (DayState.Closed):
                    tmpUI.enabled = true;
                    tmpUI.text = "CLOCK OUT TO END DAY";
                    break;
                default:
                    tmpUI.enabled = false;
                    break;
            }
        }
    }

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
            UI_ItemDisplay orderListItemUI = Instantiate(orderListItemPrefab, orderListParent);
            orderListItemUI.InitializeItem(item);
        }

        timerThrobberImage.sprite = timerThrobberSprite;
    }

    public void ClearOrder(Order order)
    {
        order.OnOrderRemoved -= ClearOrder;
        order.OnOrderTimerUpdate -= OnTimerUpdate;

        ClearList();

        timerThrobberImage.fillAmount = 1;
        timerThrobberImage.sprite = timerWaitingSprite;
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
        if (timerThrobberImage != null)
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

    private void OnDestroy()
    {
        GameStateManager.OnDayStateChanged -= UpdateScreenText;
    }
}
