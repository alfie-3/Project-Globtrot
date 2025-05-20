using UnityEngine;

public class GlobalProcessorModifiers : MonoBehaviour
{
    public static float GasStationSpeedMultiplier = 1.0f;
    public static float CombinerSpeedMultiplier = 1.0f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        GasStationSpeedMultiplier = 1;
        CombinerSpeedMultiplier = 1;
    }

    public static void UpgradeStat(ProcessorUpgradeData data)
    {
        switch (data.Upgrade)
        {
            case ProcessorUpgrades.GasStationFillMultiplier:
                GasStationSpeedMultiplier = data.Value; break;
            case ProcessorUpgrades.CombinerSpeedMultiplier:
                CombinerSpeedMultiplier = data.Value; break;
        }
    }

}



public enum ProcessorUpgrades
{
    GasStationFillMultiplier,
    CombinerSpeedMultiplier,
}