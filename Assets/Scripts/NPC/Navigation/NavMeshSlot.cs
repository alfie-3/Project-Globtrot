using System;
using UnityEngine;

public class NavMeshSlot : MonoBehaviour
{
    public bool IsOccupied {get; private set;} = false;
    [field: SerializeField] public GameObject OccupyingGameObject { get; private set;}

    [SerializeField] NavMeshSlot nextNavmeshSlot;

    public Action<GameObject, NavMeshSlot, bool> OnOccupiedChange;
    public NavMeshSlotManager SlotManager { get; private set;}

    [SerializeField] float OccupiedCutoffRange = 3;

    private void Update()
    {
        if (!IsOccupied) return;

        if (OccupyingGameObject == null)
        {
            IsOccupied = false;
            return;
        }

        if (Vector3.Distance(OccupyingGameObject.transform.position, transform.position) > OccupiedCutoffRange)
        {
            IsOccupied = false;
        }
    }

    public void SetSlotManager(NavMeshSlotManager slotManager)
    {
        this.SlotManager = slotManager;
    }

    public void SetNextSlot(NavMeshSlot nextSlot)
    {
        nextNavmeshSlot = nextSlot;
    }

    public void Occupy(GameObject occupiedObject)
    {
        IsOccupied = true;
        OnOccupiedChange.Invoke(occupiedObject, this, true);
        OccupyingGameObject = occupiedObject;
    }

    public void UnOccupy(GameObject occupiedObject)
    {
        IsOccupied = false;
        OnOccupiedChange.Invoke(occupiedObject, this, false);
        OccupyingGameObject = null;
    }

    public bool IsFrontOfQueue()
    {
        if (SlotManager == null) { return false; }

        return SlotManager.IsAtFrontOfQueue(this);
    }

    public bool TryGetNextSlot(out NavMeshSlot nextSlot)
    {
        nextSlot = null;

        if (nextNavmeshSlot == null)
        {
            return false;
        }
        else
        {
            nextSlot = nextNavmeshSlot;
            return true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = IsOccupied ? Color.green : Color.red;

        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}
