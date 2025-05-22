using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NutrientBaby : NetworkBehaviour
{
    [SerializeField] List<Stock_Item> requestableStock;
    [SerializeField] Image brainScannerScreen;
    [SerializeField] Animator wormAnimator;
    [Space]
    [SerializeField] Transform outputSpawnPoint;
    [SerializeField] NetworkObject energyCorePrefab;
    [SerializeField] NetworkObject wastePrefab;
    [Space]
    [SerializeField] Upgrade secondBrainScannerUpgrade;
    [SerializeField] Image upgradeBrainScannerScreen;

    NetworkVariable<FixedString32Bytes> nextrequestedItemID = new();
    NetworkVariable<FixedString32Bytes> requestedItemID = new();
    Stock_Item requestedItem;

    public Action<bool> OnFeed = delegate { };
    public Action<Stock_Item> OnNewItemAssigned = delegate { };

    private void Awake() {
        requestedItemID.OnValueChanged += (prev, current) => AssignItem(current);
        OnNewItemAssigned += UpdateBrainScanner;
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            Stock_Item newItem = requestableStock[UnityEngine.Random.Range(0, requestableStock.Count)];
            nextrequestedItemID.Value = newItem.ItemID;
            GenerateNewItem();
        }
        else
        {
            AssignItem(requestedItemID.Value);
        }

        if (UpgradesManager.Instance.CurrentUpgrades.Contains(secondBrainScannerUpgrade))
            AddSecondScreen();
        else
        {
            UpgradesManager.OnUnlockedUpgrade += onUpgradeUnlocked;
        }

    }

    public bool TryFeed(NetworkObjectReference feedingItem, Stock_Item inputItem) {
        if (!requestableStock.Contains(inputItem))
        {
            return false;
        }

        if (inputItem.ItemID.Equals(requestedItemID.Value.ToString())) {
            Feed_Rpc(feedingItem, true);
        }
        else {
            Feed_Rpc(feedingItem, false);
        }

        if (IsServer)
        {
            GenerateNewItem();
        }

        return true;
    }
    
    public bool CheckStock(Stock_Item item)
    {
        return requestableStock.Contains(item);
    }

    [Rpc(SendTo.Everyone)]
    public void Feed_Rpc(NetworkObjectReference feedingItem, bool success)
    {
        if (success)
        {
            OnFeed.Invoke(true);
            wormAnimator.CrossFade("Eat", 0.1f);

            if (IsServer)
            {
                NetworkObject newNwObj = Instantiate(energyCorePrefab, outputSpawnPoint.position, outputSpawnPoint.rotation);
                newNwObj.Spawn();
            }
        }
        else
        {
            OnFeed.Invoke(false);
            wormAnimator.CrossFade("Eat Sad", 0.1f);

            if (IsServer)
            {
                NetworkObject newNwObj = Instantiate(wastePrefab, outputSpawnPoint.position, outputSpawnPoint.rotation);
                newNwObj.Spawn();
            }
        }

        if (IsServer)
        {
            if (feedingItem.TryGet(out NetworkObject nwObj))
            {
                nwObj.Despawn();
            }
        }
    }

    public void GenerateNewItem() {
        requestedItemID.Value = nextrequestedItemID.Value;
        Stock_Item newItem = requestableStock[UnityEngine.Random.Range(0, requestableStock.Count)];
        nextrequestedItemID.Value = newItem.ItemID;
    }

    public void AssignItem(FixedString32Bytes current) {
        requestedItem = ItemDictionaryManager.RetrieveItem(current.ToString()) as Stock_Item;
        OnNewItemAssigned.Invoke(requestedItem);
    }

    public void UpdateBrainScanner(Stock_Item item) {
        if (!brainScannerScreen) return;
        
        brainScannerScreen.sprite = item.ItemIcon;
    }


    private void onUpgradeUnlocked(Upgrade upgrade)
    {
        if (upgrade == secondBrainScannerUpgrade)
        {
            AddSecondScreen();
            UpgradesManager.OnUnlockedUpgrade -= onUpgradeUnlocked;
        }
    }

    private void AddSecondScreen()
    {
        upgradeBrainScannerScreen.transform.parent.gameObject.SetActive(true);
        upgradeBrainScannerScreen.sprite = ItemDictionaryManager.RetrieveItem(nextrequestedItemID.Value.ToString()).ItemIcon;
        nextrequestedItemID.OnValueChanged += (prev, current) =>
        {
            upgradeBrainScannerScreen.sprite = ItemDictionaryManager.RetrieveItem(current.ToString()).ItemIcon;
        };
    }


    private void OnDestroy()
    {
        UpgradesManager.OnUnlockedUpgrade -= onUpgradeUnlocked;
    }
}

