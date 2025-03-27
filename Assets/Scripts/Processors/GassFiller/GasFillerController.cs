using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class GasFillerController : MonoBehaviour
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
        if (gasFillTweener != null) return;
        if (port.Filled.Value == false) return;

        gasFillTweener = DOVirtual.Float(0, 1, fillSpeed, fill => UpdateFillAmount(fill));
        gasFillTweener.onComplete += () => OnGasFillComplete(gasType);
    }

    public void UpdateFillAmount(float amount)
    {
        currentFillAmount = amount;
        OnFillAmountChanged(currentFillAmount);
    }

    public void OnGasFillComplete(GasType gasType)
    {
        gasFillTweener = DOTween.Shake(() => hackyShakeFloat, x => { hackyShakeFloat = x; UpdateFillAmount(1 + hackyShakeFloat.x); }, overPressureTimeout, 0.2f, 20, 1, false, false);

        port.SetGasCannisterType(gasTypes[(int)gasType]);
    }

    public void OnCannisterRemoved()
    {
        gasFillTweener.Kill();
        gasFillTweener = null;
        DOVirtual.Float(currentFillAmount, 0, 0.5f, fill => UpdateFillAmount(fill));
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