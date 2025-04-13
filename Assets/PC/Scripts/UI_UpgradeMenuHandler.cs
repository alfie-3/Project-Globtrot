using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_UpgradeMenuHandler : MonoBehaviour
{
    [SerializeField] Transform upgradeButtonSpawnPoint;
    [SerializeField] GameObject upgradeButtonPrefab;
    [Space]
    [SerializeField] TextMeshProUGUI upgradeTitle;
    [SerializeField] TextMeshProUGUI upgradeDescription;
    [SerializeField] Button purchaseUpgradeButton;

    Upgrade selectedUpgrade;
    Dictionary<string, GameObject> upgradeButtonDict = new();


    private void Start()
    {
        upgradeTitle.text = string.Empty;
        upgradeDescription.text = string.Empty;
        purchaseUpgradeButton.interactable = false;
    }

    private void OnEnable()
    {
        UpgradesManager.OnAddedUpgrade += AddUpgrade;
        UpgradesManager.OnUnlockedUpgrade += RemoveUpgrade;
    }

    private void OnDisable()
    {
        UpgradesManager.OnAddedUpgrade -= AddUpgrade;
        UpgradesManager.OnUnlockedUpgrade -= RemoveUpgrade;
    }

    public void AddUpgrade(Upgrade upgrade)
    {
        UI_UpgradeButton upgradeButton = Instantiate(upgradeButtonPrefab, upgradeButtonSpawnPoint).GetComponent<UI_UpgradeButton>();
        upgradeButton.GetComponent<UI_UpgradeButton>().InitButton(upgrade, this);

        upgradeButtonDict.Add(upgrade.UpgradeId, upgradeButton.gameObject);
    }

    public void RemoveUpgrade(Upgrade upgrade)
    {
        if (upgradeButtonDict.TryGetValue(upgrade.UpgradeId, out GameObject gameObject))
        {
            Destroy(gameObject);
        }

        if (selectedUpgrade == upgrade)
        {
            upgradeTitle.text = string.Empty;
            upgradeDescription.text = string.Empty;
            purchaseUpgradeButton.interactable = false;
        }
    }

    public void DisplayUpgrade(Upgrade upgrade)
    {
        upgradeTitle.text = upgrade.UpgradeName;
        upgradeDescription.text = upgrade.UpgradeDescription;

        purchaseUpgradeButton.interactable = true;

        selectedUpgrade = upgrade;
    }

    public void PurchaseUpgrade()
    {
        if (selectedUpgrade == null) return;
        if (MoneyManager.Instance?.Chips.Value - selectedUpgrade.UpgradeCost < 0) return;

        purchaseUpgradeButton.interactable = false;
        UpgradesManager.Instance.TryPurchaseUpgrade_Rpc(selectedUpgrade.UpgradeId);
    }

    private void OnDestroy()
    {
        UpgradesManager.OnAddedUpgrade -= AddUpgrade;
        UpgradesManager.OnUnlockedUpgrade -= RemoveUpgrade;
    }
}
