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
    [SerializeField] TextMeshProUGUI noWastageBonusText;
    [SerializeField] TextMeshProUGUI chipsEarnedText;
    [Space]
    [SerializeField] GameObject[] rankImages;
    [Space]
    [SerializeField] Button NextDayButton;

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

        if (!IsServer) return;

        NextDayButton.interactable = true;
    }

    public void EndDaySequence()
    {
        dayEndText.text = $"DAY {GameStateManager.Instance.CurrentDay.Value + 1}";

        Sequence sequence = DOTween.Sequence();

        sequence.Append(DOVirtual.Int(0, MoneyManager.Instance.CurrentQuotaAmount.Value, 1, (value) => rawProfitsText.text = $"Order Profits - <sprite=0>{value}"));
        sequence.Append(DOVirtual.Int(0, MoneyManager.Instance.TimeBonus.Value, 1, (value) => speedBonusText.text = $"Speed Bonus - <sprite=0>{value}"));
        sequence.Append(DOVirtual.Int(0, MoneyManager.Instance.GetTotal(), 2, (value) => speedBonusText.text = $"Total - <sprite=0>{value}"));
    }

    public void GoToNextDay()
    {
        if (!IsServer) return;

        GameStateManager.Instance.NewDay();
    }

    private void OnDisable()
    {
        CursorUtils.LockAndHideCusor();
    }
}
