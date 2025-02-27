using System;
using UnityEngine;

public static class MoneyFormatter
{
    public static string FormatPriceInt(int price)
    {
        return Math.Floor((decimal)(price / 100)).ToString("N2");
    }
}
