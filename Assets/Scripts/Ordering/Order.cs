using System.Collections.Generic;
using System.Linq;
using System;
using Unity.Netcode;
using Unity.Collections;

public class Order
{
    public int OrderId;

    public float InitialOrderTime = 30;
    public float CurrentOrderTime = 30;

    public OrderItem[] OrderItems;

    public Action<float, float> OnOrderTimerUpdate = delegate { };
    public Action<Order> OnTimerFinished = delegate { }; 

    public Action<Order> OnOrderRemoved = delegate { };
    public Action<Order> OnOrderSucceeded = delegate { };
    public Action<Order> OnOrderFailed = delegate { };

    public Order(float orderTime, List<OrderItem> items, int orderId)
    {
        InitialOrderTime = orderTime;
        CurrentOrderTime = InitialOrderTime;

        OrderItems = items.ToArray();
        OrderId = orderId;
    }

    public void UpdateTimer(float deltaTime)
    {
        CurrentOrderTime -= deltaTime;

        if (CurrentOrderTime <= 0)
        {
            OnTimerFinished.Invoke(this);
            return;
        }

        OnOrderTimerUpdate.Invoke(InitialOrderTime, CurrentOrderTime);
    }

    public OrderResponse CompareContents(Contents contents)
    {
        Contents incorrectItems = new(0);
        Contents missingItems = new(0);
        Contents extraItems = new(0);

        foreach (OrderItem orderItem in OrderItems)
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
            OrderItem foundItem = OrderItems.First(x => contentItem.Key == x.Item.ItemID);
            if (foundItem != null) continue;

            incorrectItems.TryAddItem(contentItem.Key, contentItem.Value);
        }

        return new(incorrectItems, missingItems, extraItems);
    }
}

public struct OrderResponse
{
    public ResponseStatus ResponseStatus;

    public Contents IncorrectItems;
    public Contents ExtraItems;
    public Contents MissingItems;

    public OrderResponse(Contents incorrectItems, Contents missingItems, Contents extraItems)
    {
        IncorrectItems = incorrectItems;
        MissingItems = missingItems;
        ExtraItems = extraItems;

        if (incorrectItems.Count > 0 || missingItems.Count > 0 || extraItems.Count > 0)
            ResponseStatus = ResponseStatus.Failure;
        else
            ResponseStatus = ResponseStatus.Success;
    }

    public OrderResponse(ResponseStatus responseStatus)
    {
        ResponseStatus = responseStatus;
        IncorrectItems = null;
        ExtraItems = null;
        MissingItems = null;
    }
}

public enum ResponseStatus
{
    Success,
    Failure,
    Timeout
}

public class OrderItem
{
    private string itemID;
    public ShopProduct_Item Item;

    public int Quantity = 1;

    public OrderItem(string item, int quantity)
    {
        itemID = item;

        Item = ItemDictionaryManager.RetrieveItem(item) as ShopProduct_Item;

        Quantity = quantity;
    }

    public OrderItem() { }

    public OrderItemPayload CreateOrderItemPayload()
    {
        return new(itemID, Quantity);
    }
}

public struct OrderPayload : INetworkSerializable
{
    public int OrderID;
    public OrderItemPayload[] OrderItems;
    public int AssignedPort;
    public float Time;

    public OrderPayload(Order order, int port)
    {
        Time = order.InitialOrderTime;
        OrderID = order.OrderId;

        OrderItems = new OrderItemPayload[order.OrderItems.Length];

        for (int i = 0; i < order.OrderItems.Length; i++)
        {
            OrderItems[i] = order.OrderItems[i].CreateOrderItemPayload();
        }

        AssignedPort = port;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();

            reader.ReadValueSafe(out OrderItems);
            reader.ReadValueSafe(out Time);
            reader.ReadValueSafe(out OrderID);

        }
        else
        {
            var writer = serializer.GetFastBufferWriter();

            writer.WriteValueSafe(OrderItems);
            writer.WriteValueSafe(Time);
            writer.WriteValueSafe(OrderID);
        }
    }

    public List<OrderItem> OrderItemsToList()
    {
        List<OrderItem> orderItems = new List<OrderItem>(OrderItems.Length);

        for (int i = 0; i < OrderItems.Length; i++)
        {
            orderItems.Add(new(OrderItems[i].ItemId, OrderItems[i].Quantity));
        }

        return orderItems;
    }
}

public struct OrderItemPayload : INetworkSerializable
{
    public string ItemId;
    public int Quantity;

    public OrderItemPayload(string itemID, int quantity)
    {
        ItemId = itemID;
        Quantity = quantity;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();

            reader.ReadValueSafe(out Quantity);
            reader.ReadValueSafe(out ItemId);

        }
        else
        {
            var writer = serializer.GetFastBufferWriter();

            writer.WriteValueSafe(Quantity);
            writer.WriteValueSafe(ItemId);
        }
    }
}