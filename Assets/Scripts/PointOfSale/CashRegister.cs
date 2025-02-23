using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CashRegister : MonoBehaviour
{
    public static List<CashRegister> CashRegisters = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        CashRegisters = new List<CashRegister>();
    }

    private void OnEnable()
    {
        CashRegisters.Add(this);
    }

    private void OnDisable()
    {
        CashRegisters.Remove(this);
    }

    public static bool TryGetRandomCashRegister(out CashRegister cashRegister)
    {
        cashRegister = null;

        if (CashRegisters.Count == 0) return false;

        cashRegister = CashRegisters[Random.Range(0, CashRegisters.Count)];

        return true;
    }
}
