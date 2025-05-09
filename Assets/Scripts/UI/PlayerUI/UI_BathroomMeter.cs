using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_BathroomMeter : MonoBehaviour, IInitPlayerUI
{
    [SerializeField] Slider slider;

    public void Init(PlayerUI_Manager uiManager)
    {
        if (uiManager.TryGetComponent(out PlayerBathroomHandler bathroom))
        {
            bathroom.OnBathroomNeedChange += UpdateSlider;
        }
    }

    private void UpdateSlider(float obj)
    {
        slider.value = obj;
    }
}
