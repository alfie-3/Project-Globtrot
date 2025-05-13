using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Clock : MonoBehaviour
{
    [SerializeField] float Offset;
    [SerializeField] Image rotationalImage;

    RectTransform rectTransform => (RectTransform)transform;

    public void OnEnable()
    {
        GameStateManager.OnDayStateChanged += DayStateChanged;
        GameStateManager.OnGameTimeChanged += UpdateTime;

        rectTransform.anchoredPosition = new(Offset, rectTransform.anchoredPosition.y);
    }

    public void OnDisable()
    {
        GameStateManager.OnGameTimeChanged -= UpdateTime;
    }

    public void DayStateChanged(DayState dayState)
    {
        if (dayState == DayState.Open || dayState == DayState.Overtime)
        {
            rectTransform.DOAnchorPosX(0, 2).SetEase(Ease.OutExpo);
        }
        else
        {
            rectTransform.DOAnchorPosX(Offset, 2).SetEase(Ease.InExpo);
        }
    }

    public void UpdateTime(float normalisedTime)
    {
        rotationalImage.rectTransform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, 180, normalisedTime));

    }

    private void OnDestroy()
    {
        GameStateManager.OnDayStateChanged -= DayStateChanged;
        GameStateManager.OnGameTimeChanged -= UpdateTime;
    }
}
