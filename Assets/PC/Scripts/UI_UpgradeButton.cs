using TMPro;
using UnityEngine;

public class UI_UpgradeButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI upgradeTitle;
    [SerializeField] TextMeshProUGUI upgradePrice;

    Upgrade upgrade;
    UI_UpgradeMenuHandler upgradeMenu;

    public void InitButton(Upgrade upgrade, UI_UpgradeMenuHandler upgradeMenu)
    {
        upgradeTitle.text = upgrade.UpgradeName;
        upgradePrice.text = $"<sprite=0> {upgrade.UpgradeCost}";

        this.upgrade = upgrade;
        this.upgradeMenu = upgradeMenu;
    }

    public void OnClicked()
    {
        upgradeMenu.DisplayUpgrade(upgrade);
    }

}
