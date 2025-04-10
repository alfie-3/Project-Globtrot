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

        foreach (var item in contents.ContentsDictionary)
        {
            UI_ItemDisplay orderListItemUI = Instantiate(itemDisplayPrefab, listParent);
            orderListItemUI.InitializeItem(item.Key, item.Value);
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
