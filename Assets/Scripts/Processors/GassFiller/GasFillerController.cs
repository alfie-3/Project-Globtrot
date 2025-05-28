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
    public event Action<float> OnFillAmountChanged;

    Vector3 hackyShakeFloat;

    Tweener gasFillTweener;

    [SerializeField] GasFillerInputPort port;
    [Header("GasItems")]
    [SerializeField] Stock_Item[] gasTypes;

    GasType currentGasType;
    bool filledCannister;

    [Space]
    [SerializeField] AudioClip gasLoop;
    [SerializeField] AudioClip AirEscape;
    [SerializeField] AudioClip FinishedGas;

    AudioSource audioSource;

    private void Awake()
    {
        port.OnGasCannisterRemoved += OnCannisterRemoved;
        audioSource = GetComponent<AudioSource>();
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
        gasFillTweener = DOVirtual.Float(0, 1, fillSpeed * GlobalProcessorModifiers.GasStationSpeedMultiplier, fill => UpdateFillAmount(fill));
        gasFillTweener.onComplete += () => OnGasFillComplete(gasType);
        OnGasFillStart_Rpc();
    }

    public void OverPressurizeCanister()
    {
        if (port.Filled.Value == false) return;
        port.Cannister.GetComponent<GasCanister>().OverPressurize_Rpc();
    }

    public void UpdateFillAmount(float amount)
    {
        currentFillAmount = amount;
        OnFillAmountChanged(currentFillAmount);

        if (amount > 0.8f && !filledCannister && gasFillTweener != null)
        {
            filledCannister = true;
            OnGasFillComplete_Rpc();
            port.SetGasCannisterType(gasTypes[(int)currentGasType]);
        }
    }

    public void OnGasFillComplete(GasType _)
    {
        gasFillTweener = DOTween.Shake(() => hackyShakeFloat, x => { hackyShakeFloat = x; UpdateFillAmount(1 + hackyShakeFloat.x); }, (overPressureTimeout * GlobalProcessorModifiers.GasStationSpeedMultiplier), 0.2f, 20, 1, false, false);
        gasFillTweener.onComplete += () => OverPressurizeCanister();
    }

    [Rpc(SendTo.Everyone)]
    public void OnGasFillStart_Rpc()
    {
        audioSource.clip = gasLoop;
        audioSource.loop = true;
        audioSource.Play();
    }

    [Rpc(SendTo.Everyone)]
    public void OnGasFillComplete_Rpc()
    {
        audioSource.PlayOneShot(AirEscape);
        audioSource.PlayOneShot(FinishedGas);
    }

    public void OnCannisterRemoved()
    {
        if (gasFillTweener != null)
        {
            gasFillTweener.Kill();
            gasFillTweener = null;
        }

        audioSource.Stop();

        filledCannister = false;
        DOVirtual.Float(currentFillAmount, 0, 0.5f, fill => UpdateFillAmount(fill));
        audioSource.PlayOneShot(AirEscape);
    }
}

public enum GasType
{
    Oxygen = 0,
    Argon = 1,
    Helium = 2,
    Hydrogen = 3,
    Radon = 4,
    Zeeblium = 5,
    None = 6
}