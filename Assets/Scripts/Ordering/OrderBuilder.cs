using System.Collections.Generic;

public static class OrderBuilder
{
    public static Order GenerateOrder(List<OrderableList> orderables, int uniqueId)
    {
        List<OrderItem> items = new();
        float randomTime = 15;

        foreach (OrderableList orderable in orderables)
        {
            items.AddRange(orderable.PickRandom());
        }

        foreach (OrderItem item in items)
        {
            randomTime += item.TimeContribution;
        }

        return new Order(randomTime, items, uniqueId);
    }
}