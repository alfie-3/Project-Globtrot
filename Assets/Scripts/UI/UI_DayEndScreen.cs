using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UI_DayEndScreen : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI dayEndText;
    [SerializeField] Button NextDayButton;

    private void OnEnable()
    {
        CursorUtils.UnlockAndShowCursor();
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
        dayEndText.text = $"DAY {GameStateManager.Instance.CurrentDay.Value + 1} \n CLEARED";
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
