using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Notifcation : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI notifText;
    [SerializeField] RectTransform underscoreRect;

    bool notificationRunning;
    Queue<string> notificationQueue = new();

    public static UI_Notifcation Instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        Instance = null;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        notifText.rectTransform.anchoredPosition = new(400, notifText.rectTransform.anchoredPosition.y);
        underscoreRect.anchoredPosition = new(150, underscoreRect.anchoredPosition.y);

    }

    public static void EnqueueNotification(string message)
    {
        if (Instance == null) return;
        if (Instance.notificationQueue.TryPeek(out string result))
        {
            if (message == result)
                return;
        }

        Instance.notificationQueue.Enqueue(message);

        if (!Instance.notificationRunning)
        {
            Instance.RunNotification(Instance.notificationQueue.Dequeue());
        }
    }

    public void RunNotification(string message)
    {
        notificationRunning = true;

        notifText.text = message;
        Sequence textSequence = DOTween.Sequence();
        textSequence.Append(notifText.rectTransform.DOAnchorPosX(-8, 1f));
        textSequence.AppendInterval(3.5f);
        textSequence.Append(notifText.rectTransform.DOAnchorPosX(400, 1f));

        textSequence.AppendCallback(RunNextNotification);

        Sequence underscoreSequence = DOTween.Sequence();
        underscoreSequence.AppendInterval(0.5f);
        underscoreSequence.Append(underscoreRect.DOAnchorPosX(-15, 0.5f));
        underscoreSequence.AppendInterval(3f);
        underscoreSequence.Append(underscoreRect.DOAnchorPosX(150, 0.5f));
    }

    public void RunNextNotification()
    {
        if (notificationQueue.Count > 0)
        {
            RunNotification(notificationQueue.Dequeue());
            return;
        }

        notificationRunning = false;
    }
}
