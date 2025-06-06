using System.Collections.Generic;
using UnityEngine;

public class NavMeshSlotManager : MonoBehaviour
{
    [field: SerializeField] public NavMeshSlot[] NavMeshSlots {  get; private set; }
    [SerializeField] bool isQueue;

    List<NavMeshSlot> unoccupiedSlots = new List<NavMeshSlot>();

    private void OnEnable()
    {
        NavMeshSlot previousSlot = null;

        foreach (NavMeshSlot slot in NavMeshSlots)
        {
            slot.OnOccupiedChange += UpdateUnoccupiedSlots;
            unoccupiedSlots.Add(slot);
            slot.SetSlotManager(this);

            if (isQueue)
                slot.SetNextSlot(previousSlot);

            previousSlot = slot;
        }
    }

    private void OnDisable()
    {
        foreach (NavMeshSlot slot in NavMeshSlots)
        {
            slot.OnOccupiedChange -= UpdateUnoccupiedSlots;
        }
    }

    private void UpdateUnoccupiedSlots(GameObject _, NavMeshSlot slot, bool status)
    {
        if (status == true)
            unoccupiedSlots.Remove(slot);
        else
            unoccupiedSlots.Add(slot);
    }

    public bool TryGetFreeSlot(out NavMeshSlot navMeshSlot)
    {
        foreach (NavMeshSlot slot in NavMeshSlots)
        {
            if (!slot.IsOccupied)
            {
                navMeshSlot = slot;
                return true;
            }
        }

        navMeshSlot = null;
        return false;
    }

    public bool TryGetRandomSlot(out NavMeshSlot slot)
    {
        slot = null;

        if (unoccupiedSlots.Count == 0) return false;

        slot = unoccupiedSlots[Random.Range(0, unoccupiedSlots.Count)];

        return true;

    }

    public bool IsAtFrontOfQueue(NavMeshSlot occupiedSlot) => (occupiedSlot == NavMeshSlots[0]);
}
