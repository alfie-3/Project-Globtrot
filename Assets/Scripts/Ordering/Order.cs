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

        int profits = 0;
        int loss = 0;

        foreach (OrderItem orderItem in OrderItems)
        {
            if (contents.ContentsDictionary.TryGetValue(orderItem.Item, out int quantity))
            {
                if (orderItem.Quantity == quantity)
                {
                    profits += orderItem.Item.Price * quantity;
                }
                else if (orderItem.Quantity > quantity)
                {
                    extraItems.TryAddItem(orderItem.Item, orderItem.Quantity - quantity);

                    profits += orderItem.Item.Price * orderItem.Quantity;
                    loss += orderItem.Item.Price * (quantity - orderItem.Quantity);
                }
                else if (orderItem.Quantity < quantity)
                {
                    missingItems.TryAddItem(orderItem.Item, quantity - orderItem.Quantity);

                    profits += orderItem.Item.Price * quantity;
                    loss += orderItem.Item.Price * (orderItem.Quantity - quantity);
                }
            }
            else
            {
                missingItems.TryAddItem(orderItem.Item, quantity);
                loss += orderItem.Item.Price * quantity;
            }
        }

        foreach (KeyValuePair<Stock_Item, int> contentItem in contents.ContentsDictionary)
        {
            OrderItem foundItem = OrderItems.FirstOrDefault(x => contentItem.Key == x.Item);
            if (foundItem != default) continue;

            incorrectItems.TryAddItem(contentItem.Key, contentItem.Value);
        }

        return new(incorrectItems, missingItems, extraItems, profits, loss);
    }
}

public struct OrderResponse
{
    public ResponseStatus ResponseStatus;

    public Contents IncorrectItems;
    public Contents ExtraItems;
    public Contents MissingItems;

    public int Profit;
    public int Loss;

    public OrderResponse(Contents incorrectItems, Contents missingItems, Contents extraItems, int profits, int loss)
    {
        IncorrectItems = incorrectItems;
        MissingItems = missingItems;
        ExtraItems = extraItems;
        Profit = profits;
        Loss = loss;

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
        Profit = 0;
        Loss = 0;
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
    public Stock_Item Item;

    List<object> data;
    public int Quantity = 1;

    public float TimeContribution;

    public OrderItem(Stock_Item item, int quantity, float timeContribution = 0)
    {
        itemID = item.ItemID;

        Item = item;

        Quantity = quantity;
        TimeContribution = timeContribution;
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
            reader.ReadValueSafe(out AssignedPort);

        }
        else
        {
            var writer = serializer.GetFastBufferWriter();

            writer.WriteValueSafe(OrderItems);
            writer.WriteValueSafe(Time);
            writer.WriteValueSafe(OrderID);
            writer.WriteValueSafe(AssignedPort);
        }
    }

    public List<OrderItem> OrderItemsToList()
    {
        List<OrderItem> orderItems = new List<OrderItem>(OrderItems.Length);

        for (int i = 0; i < OrderItems.Length; i++)
        {
            Stock_Item stockitem = (Stock_Item)ItemDictionaryManager.RetrieveItem(OrderItems[i].ItemId);
            if (stockitem == null) continue;

            orderItems.Add(new(stockitem, OrderItems[i].Quantity));
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