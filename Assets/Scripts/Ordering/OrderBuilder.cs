using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Netcode;
using UnityEngine.UIElements;
using Unity.Collections;

public static class OrderBuilder
{
    public static Order GenerateOrder(CurrentOrderables orderables, FixedString64Bytes uniqueId)
    {
        List<OrderItem> items = orderables.PickRandom(3);
        int randomTime = UnityEngine.Random.Range(orderables.MinMaxTime.x, orderables.MinMaxTime.y);

        return new Order(randomTime, items, uniqueId);
    }
}