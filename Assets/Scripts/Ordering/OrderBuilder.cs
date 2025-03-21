using System.Collections.Generic;

public static class OrderBuilder
{
    public static Order GenerateOrder(CurrentOrderables orderables, int uniqueId)
    {
        List<OrderItem> items = orderables.PickRandom(3);
        int randomTime = UnityEngine.Random.Range(orderables.MinMaxTime.x, orderables.MinMaxTime.y);

        return new Order(randomTime, items, uniqueId);
    }
}