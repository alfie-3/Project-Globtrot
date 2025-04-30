using DG.Tweening;
using TMPro;
using UnityEngine;

public class UI_Notifcation : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI notifText;
    [SerializeField] RectTransform underscoreRect;

    private void Awake()
    {
        notifText.rectTransform.anchoredPosition = new(400, notifText.rectTransform.anchoredPosition.y);
        underscoreRect.anchoredPosition = new(150, underscoreRect.anchoredPosition.y);

    }

    private void OnEnable()
    {
        OrderManager.OnNewOrderAdded += NewOrderNotif;
    }

    private void OnDisable()
    {
        OrderManager.OnNewOrderAdded -= NewOrderNotif;
    }

    public void NewOrderNotif(Order order, int id)
    {
        RunNotification("NEW ORDER");
    }

    public void RunNotification(string message)
    {
        Sequence textSequence = DOTween.Sequence();
        textSequence.Append(notifText.rectTransform.DOAnchorPosX(-8, 1f));
        textSequence.AppendInterval(3.5f);
        textSequence.Append(notifText.rectTransform.DOAnchorPosX(400, 1f));

        Sequence underscoreSequence = DOTween.Sequence();
        underscoreSequence.AppendInterval(0.5f);
        underscoreSequence.Append(underscoreRect.DOAnchorPosX(-15, 0.5f));
        underscoreSequence.AppendInterval(3f);
        underscoreSequence.Append(underscoreRect.DOAnchorPosX(150, 0.5f));
    }

    private void OnDestroy()
    {
        OrderManager.OnNewOrderAdded -= NewOrderNotif;
    }
}
