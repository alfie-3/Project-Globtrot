using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "PC Data/Upgrade")]
public class Upgrade : ScriptableObject
{
    [field: SerializeField] public string UpgradeId {  get; private set; }
    [field: SerializeField] public string UpgradeName { get; private set; }
    [field: TextArea(1, 6), SerializeField] public string UpgradeDescription { get; private set; }
    [field: Space]
    [field: SerializeField] public int UpgradeCost { get; private set; }
}
