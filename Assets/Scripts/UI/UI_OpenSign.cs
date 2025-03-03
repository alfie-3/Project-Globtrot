using TMPro;
using UnityEngine;

public class UI_OpenSign : MonoBehaviour
{
    [SerializeField] TMP_Text signText;

    private void OnEnable()
    {
        GameStateManager.OnShopOpenChanged += UpdateText;
    }

    private void OnDisable()
    {
        GameStateManager.OnShopOpenChanged -= UpdateText;
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
        GameStateManager.OnShopOpenChanged -= UpdateText;
    }
}
