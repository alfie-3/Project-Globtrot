using UnityEngine;

[CreateAssetMenu(fileName = "New Day Data", menuName = "Day Data")]
public class DayData : ScriptableObject
{
    [field:SerializeField] public int DailyQuota { get; private set; }
}
