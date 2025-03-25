using UnityEngine;

public class GasFillerController : MonoBehaviour
{
    public void FillGasCanister(GasType gasType)
    {

    }
}

public enum GasType
{
    Oxygen,
    Argon,
    Helium,
    Hydrogen,
    Radon,
    Zeeblium
}