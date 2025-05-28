using System.Collections.Generic;
using UnityEngine;

public static class OrderBuilder
{
    public static Order GenerateOrder(OrderManager orderManager, List<OrderableList> orderables, int uniqueId, float timeMultiplier)
    {
        List<OrderItem> items = new();
        float randomTime = 15;

        int targetValue = (int)(MoneyManager.Instance.CurrentQuotaTarget.Value * 0.2f);
        int currentValue = 0;

        List<OrderableList> tempOrderables = new List<OrderableList>(orderables);

        while (currentValue < targetValue && tempOrderables.Count > 0)
        {
            OrderableList.Shuffle(orderables);

            for (int i = tempOrderables.Count-1; i >= 0; i--)
            {
                List<OrderItem> orderItemsToAdd = tempOrderables[i].PickRandom();

                foreach (OrderItem item in orderItemsToAdd)
                {
                    items.Add(item);
                    currentValue += item.Quantity * item.Item.Price;
                }

                tempOrderables.RemoveAt(i);
            }
        }

        while (currentValue < targetValue)
        {
            OrderItem selectedItem = items[Random.Range(0, items.Count)];
            selectedItem.Quantity++;
            currentValue += selectedItem.Item.Price;
        }

        foreach (OrderItem item in items)
        {
            randomTime += item.TimeContribution;
        }

        randomTime *= timeMultiplier;

        OrderListSorting.SortOrderItems(ref items);

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