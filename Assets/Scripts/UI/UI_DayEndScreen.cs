using DG.Tweening;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UI_DayEndScreen : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI dayEndText;
    [SerializeField] TextMeshProUGUI rawProfitsText;
    [SerializeField] TextMeshProUGUI speedBonusText;
    [SerializeField] TextMeshProUGUI totalText;
    [SerializeField] TextMeshProUGUI QuotaAchievedStatusText;
    [SerializeField] TextMeshProUGUI chipsEarnedText;
    [Space]
    [SerializeField] GameObject[] rankImages;
    [Space]
    [SerializeField] Button NextDayButton;

    public Sequence sequence;

    bool continueToNextDay = false;

    private void OnEnable()
    {
        CursorUtils.UnlockAndShowCursor();

        foreach (var image in rankImages)
        {
            image.SetActive(false);
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        EndDaySequence();
    }

    public void EndDaySequence()
    {
        QuotaAchievedStatusText.enabled = false;

        dayEndText.text = $"DAY {GameStateManager.Instance.CurrentDay.Value + 1}";

        sequence = DOTween.Sequence();

        sequence.Append(DOVirtual.Int(0, MoneyManager.Instance.CurrentQuotaAmount.Value, 1, UpdateOrderProfits).SetEase(Ease.OutExpo).OnComplete(UpdateOrderProfitsQuotaFailed)).OnComplete(UpdateNextDayButton);
        sequence.Append(DOVirtual.Int(0, MoneyManager.Instance.TimeBonus.Value, 1, (value) => speedBonusText.text = $"Speed Bonus - <sprite=0>{value}").SetEase(Ease.OutExpo));
        sequence.Append(DOVirtual.Int(0, MoneyManager.Instance.GetTotal(), 2, (value) => totalText.text = $"Total - <sprite=0>{value}").SetEase(Ease.OutExpo));

        int chipsEarned = (int)(MoneyManager.Instance.GetTotal() * MoneyManager.ChipsMultiplier);
        if (!MoneyManager.Instance.MetQuota)
        {
            chipsEarned = (int)(chipsEarned * 0.12f);
        }

        MoneyManager.Instance.AddChips(chipsEarned);

        sequence.Append(DOVirtual.Int(0, chipsEarned, 1, (value) => chipsEarnedText.text = $"Chips Earned - {value}").SetEase(Ease.OutExpo));
    }

    public void UpdateOrderProfits(int value)
    {
        rawProfitsText.text = $"Order Profits - <sprite=0>{value}/{MoneyManager.Instance.CurrentQuotaTarget.Value}";

        if (value >= MoneyManager.Instance.CurrentQuotaTarget.Value && !continueToNextDay)
        {
            continueToNextDay = true;
            QuotaAchievedStatusText.enabled = true;
            QuotaAchievedStatusText.text = "QUOTA ACHIEVED!";
        }
    }

    public void UpdateOrderProfitsQuotaFailed()
    {
        if (!MoneyManager.Instance.MetQuota)
        {
            QuotaAchievedStatusText.enabled = true;
            QuotaAchievedStatusText.text = "QUOTA NOT REACHED!";
        }
    }

    public void UpdateNextDayButton()
    {
        if (!MoneyManager.Instance.MetQuota)
        {
            NextDayButton.GetComponentInChildren<TextMeshProUGUI>().text = "TRY AGAIN";
        }

        if (!IsServer) return;

        NextDayButton.interactable = true;
    }

    public void GoToNextDay()
    {
        if (!IsServer) return;

        if (continueToNextDay)
        {
            GameStateManager.Instance.NewDay();
        }
        else
        {
            GameStateManager.Instance.RepeatDay();
        }
    }

    private void OnDisable()
    {
        CursorUtils.LockAndHideCusor();
    }
}
