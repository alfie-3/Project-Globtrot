using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

//manages shop logic + basket
//split into two scripts later - one for shop another for basket stuff
public class UI_StockShop : MonoBehaviour
{
    [SerializeField] private List<GameObject> spawnedProducts = new List<GameObject>();

    [SerializeField] private MoneyManager moneyScript;
    [SerializeField] private TMP_Text currentMoneyTXT; 
    [SerializeField] private Transform spawnArea; // where purchased products will appear 

    
    public Dictionary<string, GameObject> productPrefabs = new Dictionary<string, GameObject>();

    
    

    // registers given prefab into dictionary
    public void Register(string productName, GameObject productPrefab)
    {
        if (!productPrefabs.ContainsKey(productName))
        {
            productPrefabs.Add(productName, productPrefab);
        }
        currentMoneyTXT.text = moneyScript.startingMoney.ToString();
    }

    // spawns product in spawn area
    public void SummonProduct(string productName)
    {
        if (!productPrefabs.ContainsKey(productName)) return;

        GameObject productPrefab = productPrefabs[productName];
        Vector3 spawnPos = spawnArea.position + new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        GameObject spawnedProduct = Instantiate(productPrefab, spawnPos, Quaternion.identity);
        spawnedProducts.Add(spawnedProduct);
    }

}
