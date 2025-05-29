using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class OrderBuilder
{
    public static Order GenerateOrder(OrderManager orderManager, List<OrderableList> orderables, int uniqueId, float timeMultiplier)
    {
        List<OrderItem> items = new();
        float randomTime = 15;

        int targetValue = (int)(MoneyManager.Instance.CurrentQuotaTarget.Value * orderManager.OrderValueTargetCurve.Evaluate(GameStateManager.Instance.CurrentNormalisedTime));
        int currentValue = 0;


        while (currentValue < targetValue)
        {
            OrderableList.Shuffle(orderables);

            for (int i = orderables.Count - 1; i >= 0; i--)
            {
                List<OrderItem> orderItemsToAdd = orderables[i].PickRandom();

                foreach (OrderItem item in orderItemsToAdd)
                {
                    OrderItem duplicateItem = items.FirstOrDefault(x => x.Item == item.Item);

                    if (duplicateItem != default)
                    {
                        duplicateItem.Quantity += item.Quantity;
                    }
                    else
                    {
                        items.Add(item);
                    }

                    currentValue += item.Quantity * item.Item.Price;
                }
            }
        }

        while (currentValue < targetValue)
        {
            OrderItem selectedItem = items[Random.Range(0, items.Count)];
            selectedItem.Quantity++;
            currentValue += selectedItem.Item.Price;
        }

        while (currentValue > targetValue)
        {
            OrderItem selectedItem = items[Random.Range(0, items.Count)];

            selectedItem.Quantity--;
            currentValue -= selectedItem.Item.Price;

            if (selectedItem.Quantity == 0)
                items.Remove(selectedItem);
        }

        foreach (OrderItem item in items)
        {
            randomTime += item.TimeContribution;
        }

        randomTime *= timeMultiplier;

        OrderListSorting.SortOrderItems(ref items);

        Debug.Log($"Current - {currentValue}, Target - {targetValue}");

        return new Order(randomTime, items, uniqueId);
    }

    public static int GetOrderTarget(OrderManager orderManager)
    {
        float quotaRatio = orderManager.OrderValueTargetCurve.Evaluate(GameStateManager.Instance != null ? GameStateManager.Instance.CurrentNormalisedTime : 0.5f);
        int target = (int)(MoneyManager.Instance.CurrentQuotaTarget.Value * quotaRatio);
        target *= (int)Random.Range(1, 1.15f);

        return target;
    }
}