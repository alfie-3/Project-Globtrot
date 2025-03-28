using DG.Tweening;
using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class GasFillerController : NetworkBehaviour
{
    [SerializeField] float fillSpeed = 10;
    [SerializeField] float overPressureTimeout = 5;

    float currentFillAmount;
    public Action<float> OnFillAmountChanged;

    Vector3 hackyShakeFloat;

    Tweener gasFillTweener;

    [SerializeField] GasFillerInputPort port;
    [Header("GasItems")]
    [SerializeField] Stock_Item[] gasTypes;

    GasType currentGasType;
    bool filledCannister;

    [Space]
    [SerializeField] AudioClip AirEscape;

    private void Awake()
    {
        port.OnGasCannisterRemoved += OnCannisterRemoved;
    }

    public void FillGasCannister(int gasType)
    {
        FillGasCanister((GasType)gasType);
    }

    public void FillGasCanister(GasType gasType)
    {
        if (!IsServer) return;

        if (gasFillTweener != null) return;
        if (port.Filled.Value == false) return;

        currentGasType = gasType;
        gasFillTweener = DOVirtual.Float(0, 1, fillSpeed, fill => UpdateFillAmount(fill));
        gasFillTweener.onComplete += () => OnGasFillComplete(gasType);
    }

    public void UpdateFillAmount(float amount)
    {
        currentFillAmount = amount;
        OnFillAmountChanged(currentFillAmount);

        if (amount > 0.8f && !filledCannister && gasFillTweener != null)
        {
            filledCannister = true;
            port.SetGasCannisterType(gasTypes[(int)currentGasType]);
        }
    }

    public void OnGasFillComplete(GasType gasType)
    {
        gasFillTweener = DOTween.Shake(() => hackyShakeFloat, x => { hackyShakeFloat = x; UpdateFillAmount(1 + hackyShakeFloat.x); }, overPressureTimeout, 0.2f, 20, 1, false, false);
        filledCannister = false;
    }

    public void OnCannisterRemoved()
    {
        gasFillTweener.Kill();
        gasFillTweener = null;
        filledCannister = false;
        DOVirtual.Float(currentFillAmount, 0, 0.5f, fill => UpdateFillAmount(fill));
        GetComponent<AudioSource>().PlayOneShot(AirEscape);
    }
}

public enum GasType
{
    Oxygen = 0,
    Argon = 1,
    Helium = 2,
    Hydrogen = 3,
    Radon = 4,
    Zeeblium = 5
}