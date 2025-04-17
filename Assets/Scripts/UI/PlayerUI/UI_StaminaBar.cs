using UnityEngine;
using UnityEngine.UI;

public class UI_StaminaBar : MonoBehaviour, IInitPlayerUI
{
    [SerializeField] Slider StaminaBarSlider;

    public void Init(PlayerUI_Manager uiManager)
    {
        if (uiManager.TryGetComponent(out PlayerCharacterController characterController))
        {
            characterController.Stamina.OnStaminaUpdated += UpdateStaminaBar;
        }
    }

    public void UpdateStaminaBar(float normalisedStaminaValue)
    {
        StaminaBarSlider.value = normalisedStaminaValue;
    }
}
