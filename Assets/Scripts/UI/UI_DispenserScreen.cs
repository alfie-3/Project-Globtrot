using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_DispenserScreen : MonoBehaviour
{
    [SerializeField] ItemBase item;
    [Space]
    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI itemName;

    private void Start()
    {
        InitUI();
    }

    public void InitUI()
    {
        if (item == null)
        {
            itemImage.enabled = false;
            itemName.enabled = false;
        }

        if (item.ItemIcon)
        {
            itemImage.sprite = item.ItemIcon;
        }

        itemName.text = item.ItemName;
    }
}
