using TMPro;
using UnityEngine;

public class GasCanister : MonoBehaviour
{
    [SerializeField] TMP_Text text;

    private void Awake()
    {
        GetComponent<StockItem>().OnItemChanged += UpdateGasType;
    }

    public void UpdateGasType(Stock_Item item)
    {
        string name = item.ItemName;
        name = name.Replace(" Canister", "");
        text.text = name;
    }
}
