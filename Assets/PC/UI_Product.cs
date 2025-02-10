using UnityEngine;
using TMPro;


public class UI_Product : MonoBehaviour
{
    [SerializeField] TMP_Text productAmount;
    [SerializeField] TMP_Text productName;

    public void UpdateProduct(string newName, string newAmount)
    {
        productAmount.text = newAmount;
        productName.text = newName;
    }

    public void Trash()
    {
        //get rid from basket +
        Destroy(gameObject);
    }
}
