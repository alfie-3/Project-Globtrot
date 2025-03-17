using Unity.Netcode;
using UnityEngine;

public class PlayerObjectSocketManager : NetworkBehaviour
{
    public enum ObjectSocket
    {
        Chest,
        LeftHand,
        RightHand,
    }

    [SerializeField] HoldingItemSocket ChestSlot;
    [SerializeField] HoldingItemSocket LeftHandSlot;
    [SerializeField] HoldingItemSocket RightHandSlot;

    [Rpc(SendTo.Everyone)]
    public void BindObject_Rpc(ObjectSocket slot, NetworkObjectReference networkObjectReference)
    {
        switch (slot)
        {
            case ObjectSocket.LeftHand:
                LeftHandSlot.BindObject(networkObjectReference); break;
            case ObjectSocket.RightHand:
                RightHandSlot.BindObject(networkObjectReference); break;
            case ObjectSocket.Chest:
                ChestSlot.BindObject(networkObjectReference); break;
        }
    }

    [Rpc(SendTo.Everyone)]  
    public void ClearBoundObject_Rpc(ObjectSocket slot, Vector3 position, Quaternion rotation, bool resetVelocity = false)
    {
        switch (slot)
        {
            case ObjectSocket.LeftHand:
                LeftHandSlot.ClearObjectBinding(position, rotation, resetVelocity); break;
            case ObjectSocket.RightHand:
                RightHandSlot.ClearObjectBinding(position, rotation, resetVelocity); break;
            case ObjectSocket.Chest:
                ChestSlot.ClearObjectBinding(position, rotation, resetVelocity); break;
        }
    }

    public void ClearAllBoundObjects()
    {
        ChestSlot.ClearBoundObject();
        LeftHandSlot.ClearBoundObject();
        RightHandSlot.ClearBoundObject();
    }
}
