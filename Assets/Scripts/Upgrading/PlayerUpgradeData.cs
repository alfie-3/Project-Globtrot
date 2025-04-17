using UnityEngine;

[CreateAssetMenu(fileName = "New Player Upgrade", menuName = "Upgrades/Player Upgrade")]
public class PlayerUpgradeData : ScriptableObject
{
    [field: SerializeField] public PlayerUpgrades Upgrade;
    [field: SerializeField] public float Value;
}
