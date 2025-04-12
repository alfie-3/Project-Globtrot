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

            return;
        }

        if (item.ItemIcon != null)
        {
            itemImage.sprite = item.ItemIcon;
        }
        else
        {
            itemImage.enabled = false;
        }

        itemName.text = item.ItemName;
    }
}
