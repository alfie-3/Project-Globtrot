using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Day Data", menuName = "Day Data")]
public class DayData : ScriptableObject
{
    [field:SerializeField] public int DailyQuota { get; private set; }

    [field: SerializeField] public List<Email> DayEmails { get; private set; }
    [field: SerializeField] public List<OrderableList> OrderableLists {  get; private set; }
}
