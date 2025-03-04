using UnityEngine;
using WebSocketSharp;

public abstract class ItemBase : ScriptableObject
{
    [field: SerializeField] public string ItemID { get; protected set; }

    [SerializeField] protected string itemName;
    public string ItemName
    {
        get
        {
            if (itemName.IsNullOrEmpty()) return ItemID;
            else return itemName;
        }
    }

    [field: SerializeField] public Sprite ItemIcon { get; protected set; }

    public void SetIcon(Sprite icon)
    {
        ItemIcon = icon;
    }

}
