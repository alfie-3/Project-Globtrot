using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CombinerScreen : MonoBehaviour
{
    [SerializeField]
    RectTransform Default;
    [SerializeField]
    RectTransform Ammo;
    [SerializeField]
    RectTransform PowerFrame;
    [SerializeField]
    RectTransform End;

    RectTransform _current;
    RectTransform current { get { return _current; } set { _current.gameObject.SetActive(false); _current = value; _current.gameObject.SetActive(true); } }

    private void Awake()
    {
        _current = Default;
    }
    public void ItemChanged(Stock_Item item)
    {
        if (item == null)
            current = Default; return;
        

    }

    public void newState(Stock_Item item,Stock_Item material)
    {
        if (item == null)
        {
            current = Default;
            return;
        }
        if (material == null)
        {
            if(item.ItemID == "ammocrate_base")
                current = Ammo;
            else if (item.ItemID == "powercore")
                current = PowerFrame;
            return;
        }

    }

    internal void EndScreen(Stock_Item output)
    {
        End.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = output.ItemName;
        End.GetChild(1).GetComponent<RawImage>().texture = output.ItemIcon.texture;
        current = End;
    }
}
