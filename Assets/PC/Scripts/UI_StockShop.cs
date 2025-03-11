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

    [SerializeField] private MoneyManager moneyScript;
    [SerializeField] private Transform spawnArea; // where purchased products will appear 
    [SerializeField] private GameObject boxPrefab;
    
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
    public void SummonProduct(string productName)
    {
        if (!productPrefabs.ContainsKey(productName)) return;

        Vector3 spawnPos = spawnArea.position + new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        GameObject spawnedBox = Instantiate(boxPrefab, spawnPos, Quaternion.identity);
        
        NetworkObject networkObject = spawnedBox.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn(); 
        }

        spawnedProducts.Add(spawnedBox);

        ItemHolder itemHolder = spawnedBox.GetComponent<ItemHolder>();
        StockBoxController stockBox = spawnedBox.GetComponent<StockBoxController>();

        if (itemHolder != null && stockBox != null)
        {
            if (ItemDictionaryManager.ItemDict.TryGetValue(productName, out ItemBase itemData))
            {
                if (itemData is ShopProduct_Item productItem)
                {
                    itemHolder.SetProductItem(productItem);
                }
            }
        }
    }


}
