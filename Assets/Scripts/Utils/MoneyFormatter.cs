using System;
using UnityEngine;

public static class MoneyFormatter
{
    public static string FormatPriceInt(double price)
    {
        return price.ToString("N2");
    }
}
