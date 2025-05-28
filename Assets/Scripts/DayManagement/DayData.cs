using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Day Data", menuName = "Day Data")]
public class DayData : ScriptableObject
{
    [field:SerializeField] public int DailyQuota { get; private set; }
    [field: SerializeField] public int AddedDayCoins;
    [field: Space]
    [field: SerializeField] public List<Email> DayEmails { get; private set; }
    [field :SerializeField] public List<Upgrade> AddedUpgrades { get; private set; }
    [field: Space]
    [field: SerializeField] public List<OrderableList> OrderableLists {  get; private set; }

    [field: SerializeField] public List<Placeable> AddedPlaceables { get; private set; }

    [field: SerializeField] public AudioClip Music { get; private set; }

    [System.Serializable]
    public struct Placeable
    {
        [field: SerializeField] public string category { get; private set; }
        [field: SerializeField] public PlacableFurniture_Item prefab { get; private set; }
    }

    
}