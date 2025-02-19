using UnityEngine;

public class NavMeshSlotManager : MonoBehaviour
{
    [SerializeField] private NavMeshSlot[] NavMeshSlot;

    public bool TryGetFreeSlot(out NavMeshSlot navMeshSlot)
    {
        foreach (NavMeshSlot slot in NavMeshSlot)
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
}
