using System;
using Unity.Netcode;
using UnityEngine;

public class GasFillerInputPort : NetworkBehaviour, IUseItem
{
    [SerializeField] Stock_Item referenceItem;

    public NetworkVariable<bool> Filled { get; private set; } = new();
    public Action OnGasCannisterRemoved = delegate { };

    public NetworkObject cannister;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
            Filled.Value = false;
    }

    public void OnItemUsed(PlayerHoldingManager manager, Stock_Item shopProduct_Item)
    {
        if (shopProduct_Item == referenceItem)
        {
            if (Filled.Value == true) return;

            NetworkObject heldobject = manager.HeldObj;
            manager.DisconnectHeldObject_Rpc();

            ConnectItem_Rpc(heldobject);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void ConnectItem_Rpc(NetworkObjectReference obj)
    {
        if (!obj.TryGet(out NetworkObject nwObject)) return;

        nwObject.GetComponent<Pickup_Interactable>().OnPickedUp += CannisterRemoved;
        cannister = nwObject;

        if (!IsServer) return;

        Filled.Value = true;

        nwObject.GetComponent<RigidbodyNetworkTransform>().SetRigidbodyEnabled(false);
        nwObject.GetComponent<RigidbodyNetworkTransform>().Teleport(transform.position, transform.rotation, nwObject.transform.lossyScale);
    }

    public void CannisterRemoved()
    {
        Pickup_Interactable pickup_Interactable = cannister.GetComponent<Pickup_Interactable>();
        pickup_Interactable.OnPickedUp -= CannisterRemoved;
        OnGasCannisterRemoved.Invoke();

        if (!IsServer) return;

        Filled.Value = false;
    }

    public void SetGasCannisterType(Stock_Item item)
    {
        cannister.GetComponent<StockItem>().SetItem(item);
    }
}
