using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class UI_StockShop : MonoBehaviour
{
    [SerializeField] List<GameObject> basket = new List<GameObject>(); 
    [SerializeField] Transform basketParent;
    [SerializeField] GameObject productPrefab;
    [SerializeField] Transform basketStartPos;
    int basketCounter = 0;
    int decY = 75;
    int productAmount;
    InputField inputField;

    public void UpdateAmount()
    {
        productAmount = int.Parse(inputField.text);
    }

    public void SendToBasket()
    {
        GameObject product;
        if(basketCounter == 0)
        {
            product = Instantiate(productPrefab, basketStartPos.position ,Quaternion.identity,basketParent);
            basket.Add(product);
            basketCounter++;
        }
        else if(basketCounter != 10)
        {
            Vector3 spawnPos = basketStartPos.position;
            spawnPos.y -= decY;
            product = Instantiate(productPrefab, spawnPos ,Quaternion.identity,basketParent);
            basket.Add(product);
            decY += 75;
            basketCounter++;
        }
    }


}    






