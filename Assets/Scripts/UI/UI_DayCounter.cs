using TMPro;
using UnityEngine;

public class UI_DayCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI DayText;
    [SerializeField] TextMeshProUGUI DayState;

    private void Start()
    {
        GameStateManager.Instance.CurrentDay.OnValueChanged += (prev, current) => { UpdateDayCounter(current); };
        GameStateManager.Instance.CurrentDayState.OnValueChanged += (prev, current) => { UpdateDayState(current); };
    }

    public void UpdateDayCounter(int current)
    {
        DayText.text = $"Day - {current + 1}";
    }

    public void UpdateDayState(DayState dayState)
    {
        if (dayState == global::DayState.Preperation)
        {
            DayState.text = "Preparation";
        }
        else if (dayState == global::DayState.Open)
        {
            DayState.text = "Open";
        }
        else if (dayState == global::DayState.Closed)
        {
            DayState.text = "Closed";
        }
    }
}
