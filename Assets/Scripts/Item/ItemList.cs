using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Items", menuName = "Items/Item List", order = 0)]
public class ItemList : ScriptableObject
{
    [field: SerializeField] public List<ItemBase> Items { get; private set; } = new();
}
