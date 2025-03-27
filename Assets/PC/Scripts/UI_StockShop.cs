using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using Unity.Netcode;

//manages shop logic + basket
//split into two scripts later - one for shop another for basket stuff
public class UI_StockShop : MonoBehaviour
{
    [SerializeField] private List<GameObject> spawnedProducts = new List<GameObject>();

    [SerializeField] private Transform spawnArea; // where purchased products will appear 
    [SerializeField] private GameObject itemBoxPrefab;
    [SerializeField] private GameObject furnitureBoxPrefab;
    
    public Dictionary<string, GameObject> productPrefabs = new Dictionary<string, GameObject>();
    
    // registers given prefab into dictionary
    public void Register(string productName, GameObject productPrefab)
    {
        if (!productPrefabs.ContainsKey(productName))
        {
            productPrefabs.Add(productName, productPrefab);
        }
    }

    // spawns product in spawn area + server stuff
    public void SummonItem(string productName)
    {
        if (!productPrefabs.ContainsKey(productName)) return;

        Vector3 spawnPos = spawnArea.position + new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        GameObject spawnedItem = null;

        if (ItemDictionaryManager.ItemDict.TryGetValue(productName, out ItemBase itemData))
        {
            if (itemData is Stock_Item productItem)
            {
                spawnedItem = Instantiate(itemBoxPrefab, spawnPos, Quaternion.identity);

            }
            else if (itemData is PlacableFurniture_Item furnitureItem)
            {
                spawnedItem = Instantiate(furnitureBoxPrefab, spawnPos, Quaternion.identity);
            }
        }

        NetworkObject networkObject = spawnedItem.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn();  
        }

        spawnedProducts.Add(spawnedItem);

        ItemHolder itemHolderScript = spawnedItem.GetComponent<ItemHolder>();
        FurnitureBoxController furnitureBoxControllerScript = spawnedItem.GetComponent<FurnitureBoxController>();

        if (ItemDictionaryManager.ItemDict.TryGetValue(productName, out itemData))
        {
            if (itemData is Stock_Item productItem)
            {
                itemHolderScript.AddItemServer_Rpc(itemData.ItemID, productItem.MaxInBox);
            }
            else if (itemData is PlacableFurniture_Item furnitureItem)
            {
                furnitureBoxControllerScript.SetItem_Rpc(furnitureItem.ItemID, 0f);
            }
        }
    }


}
