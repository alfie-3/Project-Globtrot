using UnityEngine;

[CreateAssetMenu(fileName = "New Processor Upgrade", menuName = "Upgrades/Processor Upgrade")]
public class ProcessorUpgradeData : ScriptableObject
{
    [field: SerializeField] public ProcessorUpgrades Upgrade;
    [field: SerializeField] public float Value;
}