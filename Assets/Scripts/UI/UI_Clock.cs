using TMPro;
using UnityEngine;

public class UI_Clock : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI clockText;

    public void OnEnable()
    {
        GameStateManager.OnGameTimeChanged += UpdateText;
    }

    public void OnDisable()
    {
        GameStateManager.OnGameTimeChanged -= UpdateText;
    }

    public void UpdateText(int currentTime)
    {
        int hours = currentTime / 60;
        int minutes = currentTime % 60;

        clockText.text = $"{hours:00} : {minutes:00}";
    }

    private void OnDestroy()
    {
        GameStateManager.OnGameTimeChanged -= UpdateText;
    }
}
