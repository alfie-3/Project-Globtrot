using UnityEngine;

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
}
