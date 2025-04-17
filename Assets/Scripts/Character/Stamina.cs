using System;
using UnityEngine;

[System.Serializable]
public class Stamina
{
    [Header("Stamina")]
    public Action<float> OnStaminaUpdated = delegate { };
    [SerializeField] float maxStamina = 5.0f;
    [SerializeField] float staminaDrainSpeed = 1;

    [SerializeField] float baseStaminaRechargeSpeed = 1;
    float staminaRechargedSpeed => baseStaminaRechargeSpeed * GlobalPlayerModifiers.StaminaRechargeSpeedModifier;

    public float CurrentStamina { get; private set; }

    public void UpdateStamina(bool consuming)
    {
        if (consuming)
        {
            CurrentStamina = Mathf.Clamp(CurrentStamina - (staminaDrainSpeed * Time.deltaTime), 0, maxStamina);
        }
        else
        {
            CurrentStamina = Mathf.Clamp(CurrentStamina + (staminaRechargedSpeed * Time.deltaTime), 0, maxStamina);
        }

        OnStaminaUpdated.Invoke(CurrentStamina / maxStamina);
    }

    public void ResetStamina()
    {
        CurrentStamina = maxStamina;
    }
}
