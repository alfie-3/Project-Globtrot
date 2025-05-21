using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class UI_DisplayContents : MonoBehaviour
{
    [SerializeField] UI_ItemDisplay itemDisplayPrefab;
    [SerializeField] Transform listParent;

    public void AddContents(Contents contents)
    {
        foreach (Transform child in listParent)
        {
            ClearList();
        }

        List<ContentsItem> contentsList = ContentsItem.GenerateContentItemsFromDict(contents.ContentsDictionary, true);

        foreach (var item in contentsList)
        {
            UI_ItemDisplay orderListItemUI = Instantiate(itemDisplayPrefab, listParent);
            orderListItemUI.InitializeItem(item.Item, item.Quantity);
        }
    }

    public void ClearList()
    {
        foreach (Transform child in listParent)
        {
            Destroy(child.gameObject);
        }
    }
}
