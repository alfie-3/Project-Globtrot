using UnityEngine;

public class GlobalProcessorModifiers : MonoBehaviour
{
    public static float GasStationSpeedMultiplier = 1.0f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        GasStationSpeedMultiplier = 1;
    }

    public static void UpgradeStat(ProcessorUpgradeData data)
    {
        switch (data.Upgrade)
        {
            case (ProcessorUpgrades.GasStationFillMultiplier):
                GasStationSpeedMultiplier = data.Value; break;
        }
    }

}

[CreateAssetMenu(fileName = "New Processor Upgrade", menuName = "Upgrades/Processor Upgrade")]
public class ProcessorUpgradeData : ScriptableObject
{
    [field: SerializeField] public ProcessorUpgrades Upgrade;
    [field: SerializeField] public float Value;
}

public enum ProcessorUpgrades
{
    GasStationFillMultiplier
}