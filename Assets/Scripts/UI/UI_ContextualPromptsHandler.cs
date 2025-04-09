using TMPro;
using UnityEngine;

public class UI_ContextualPromptsHandler : MonoBehaviour, IInitPlayerUI
{
    [SerializeField] TextMeshProUGUI InteractPrompt;
    [SerializeField] TextMeshProUGUI ThrowPrompt;
    [SerializeField] TextMeshProUGUI DropPrompt;

    private void OnEnable()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void Init(PlayerUI_Manager uiManager)
    {
        if (uiManager.TryGetComponent(out PlayerInteractionManager interactionManager))
        {
            interactionManager.OnSetObjectViewed += (value) => { TogglePrompt(value, InteractPrompt.gameObject); };
        }

        if (uiManager.TryGetComponent(out PlayerHoldingManager holdingManager))
        {
            holdingManager.NetworkedHeldObj.OnValueChanged += (prev, current) => { TogglePrompt(current.IsHolding, DropPrompt.gameObject); TogglePrompt(current.IsHolding, ThrowPrompt.gameObject); };
        }
    }

    private void TogglePrompt(bool value, GameObject prompt)
    {
        prompt.SetActive(value);
    }
}
