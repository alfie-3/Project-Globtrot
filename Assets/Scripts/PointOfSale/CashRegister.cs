using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CashRegister : MonoBehaviour
{
    public static HashSet<CashRegister> CashRegisters = new HashSet<CashRegister>();
    private static System.Random random = new System.Random();

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

        cashRegister = CashRegisters.ElementAt(random.Next(CashRegisters.Count));

        return true;
    }
}
