using TMPro;
using UnityEngine;

public class UI_OpenSign : MonoBehaviour
{
    [SerializeField] TMP_Text signText;

    private void OnEnable()
    {
        GameStateManager.OnDayStateChanged += UpdateText;
    }

    private void OnDisable()
    {
        GameStateManager.OnDayStateChanged -= UpdateText;
    }

    public void UpdateText(bool toggle)
    {
        if (toggle)
        {
            signText.text = "OPEN";
        }
        else
        {
            signText.text = "CLOSED";
        }
    }

    private void OnDestroy()
    {
        GameStateManager.OnDayStateChanged -= UpdateText;
    }
}
