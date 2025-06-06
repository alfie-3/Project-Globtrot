using System.Collections.Generic;
using UnityEngine;

public static class ItemDictionaryManager
{
    static Dictionary<string, ItemBase> ITEM_DICT;
    public static IReadOnlyDictionary<string, ItemBase> ItemDict => ITEM_DICT;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void RegisterItems()
    {
        ITEM_DICT = new Dictionary<string, ItemBase>();

        Object[] itemLists = Resources.LoadAll("Items", typeof(ItemList));

        int registeredItems = 0;
        foreach (object itemList in itemLists)
        {
            ItemList list = itemList as ItemList;
            foreach (ItemBase item in list.Items)
            {
                RegisterItem(item);
                registeredItems++;
            }
        }

        Debug.Log($"Registered {registeredItems} item(s)");
    }

    public static void RegisterItem(ItemBase item)
    {
        if (!ITEM_DICT.TryAdd(item.ItemID, item))
        {
            Debug.Log($"Error loading the item - {item.name}");
        }

        item.Init();
    }

    public static ItemBase RetrieveItem(string key)
    {
        if (ITEM_DICT.TryGetValue(key, out ItemBase item))
        {
            return item;
        }
        else
        {
            Debug.LogWarning($"Could not find the item {key}");
        }

        return null;
    }
}
