using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Day List", menuName = "Lists/Day Data")]
public class DayDataList : ScriptableObject
{
    [field: SerializeField] public List<DayData> DayList = new List<DayData>();
}
