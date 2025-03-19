using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class OrderBuilder
{
    public static Order GenerateOrder(CurrentOrderables orderables)
    {
        List<OrderItem> items = orderables.PickRandom(3);
        int randomTime = UnityEngine.Random.Range(orderables.MinMaxTime.x, orderables.MinMaxTime.y);

        return new Order(randomTime, items);
    }
}

public class Order
{
    public float OrderTime = 30;
    public List<OrderItem> orderItems;

    public Action<Order> OnOrderRemoved = delegate { };
    public Action<Order> OnOrderSucceeded = delegate { };
    public Action<Order> OnOrderFailed = delegate { };

    public Order(int orderTime, List<OrderItem> items)
    {
        OrderTime = orderTime;
        orderItems = items;
    }

    public OrderResponse CompareContents(Contents contents)
    {
        Contents incorrectItems = new(0);
        Contents missingItems = new(0);
        Contents extraItems = new(0);

        foreach (OrderItem orderItem in orderItems)
        {
            if (contents.ContentsDictionary.TryGetValue(orderItem.Item.ItemID, out int quantity))
            {
                if (orderItem.Quantity > quantity)
                {
                    extraItems.TryAddItem(orderItem.Item.ItemID, orderItem.Quantity - quantity);
                }
                else if (orderItem.Quantity < quantity)
                {
                    missingItems.TryAddItem(orderItem.Item.ItemID, quantity - orderItem.Quantity);
                }
            }
            else
            {
                missingItems.TryAddItem(orderItem.Item.ItemID, quantity);
            }
        }

        foreach (KeyValuePair<string, int> contentItem in contents.ContentsDictionary)
        {
            OrderItem foundItem = orderItems.First(x => contentItem.Key == x.Item.ItemID);
            if (foundItem != null) continue;

            incorrectItems.TryAddItem(contentItem.Key, contentItem.Value);
        }

        return new(incorrectItems, missingItems, extraItems);
    }
}

public struct OrderResponse
{
    public bool Success;

    public Contents IncorrectItems;
    public Contents ExtraItems;
    public Contents MissingItems;

    public OrderResponse(Contents incorrectItems, Contents missingItems, Contents extraItems)
    {
        IncorrectItems = incorrectItems;
        MissingItems = missingItems;
        ExtraItems = extraItems;

        if (incorrectItems.Count > 0 || missingItems.Count > 0 || extraItems.Count > 0)
            Success = false;
        else
            Success = true;
    }
}

public class OrderItem
{
    public ShopProduct_Item Item;
    public int Quantity = 1;

    public OrderItem(ShopProduct_Item item, int quantity)
    {
        Item = item;
        Quantity = quantity;
    }
}