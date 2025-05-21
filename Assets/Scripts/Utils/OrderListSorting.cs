using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class OrderListSorting
{
    public static void SortOrderItems(ref List<OrderItem> orderItems)
    {
       orderItems = orderItems.OrderByDescending(x => x.Quantity).ThenByDescending(x => x.Item.name).ToList();
    }

    public static void SortContentsItems(ref List<ContentsItem> contentItems)
    {
        contentItems = contentItems.OrderByDescending(x => x.Quantity).ThenByDescending(x => x.Item.name).ToList();
    }
}

public struct ContentsItem
{
    public Stock_Item Item;
    public int Quantity;

    public static List<ContentsItem> GenerateContentItemsFromDict(Dictionary<Stock_Item, int> dict, bool sort = false)
    {
        List<ContentsItem> contentsItems = new(dict.Count);

        foreach (KeyValuePair<Stock_Item, int> kvp in dict)
        {
            contentsItems.Add(new(kvp.Key, kvp.Value));
        }

        if (sort) OrderListSorting.SortContentsItems(ref contentsItems);

        return contentsItems;
    }

    public ContentsItem(Stock_Item item, int amount)
    {
        Item = item;
        Quantity = amount;
    }
}