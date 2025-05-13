using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class DrinksDispenser : NetworkBehaviour
{
    public static int MaxFreeDrinks = 1;
    NetworkVariable<int> FreeDrinks = new();
    [Space]
    [SerializeField] TMP_Text text;
    [SerializeField] Transform drinkSpawnTransform;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        MaxFreeDrinks = 1;
    }

    private void Awake()
    {
        GameStateManager.OnResetServer += ResetFreeDrinks;

        if (text != null)
        {
            FreeDrinks.OnValueChanged += (prev, current) => text.text = current.ToString();
        }
    }

    private void OnEnable()
    {
        if (IsServer)
        {
            ResetFreeDrinks();
        }
    }

    public void ResetFreeDrinks()
    {
        if (IsServer)
        {
            FreeDrinks.Value = MaxFreeDrinks;
        }
    }

    internal void Dispense(NetworkObject drinkDispensePrefab)
    {
        if (FreeDrinks.Value <= 0) return;

        if (IsServer)
        {
            NetworkObject nwObject = Instantiate(drinkDispensePrefab, drinkSpawnTransform.position, drinkSpawnTransform.rotation);
            nwObject.Spawn();

            FreeDrinks.Value--;
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        GameStateManager.OnResetServer -= ResetFreeDrinks;
    }
}
