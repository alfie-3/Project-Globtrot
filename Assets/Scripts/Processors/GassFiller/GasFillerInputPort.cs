using System;
using Unity.Netcode;
using UnityEngine;

public class GasFillerInputPort : NetworkBehaviour, IUseItem
{
    [SerializeField] Stock_Item referenceItem;

    public NetworkVariable<bool> Filled { get; private set; } = new();
    public Action OnGasCannisterRemoved = delegate { };

    public GameObject cannister;

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

            NetworkObject nwObj = manager.HeldObj;
            manager.ClearHeldItem(transform.position, transform.rotation.eulerAngles);
            ConnectItem_Rpc(nwObj);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void ConnectItem_Rpc(NetworkObjectReference obj)
    {
        if (!obj.TryGet(out NetworkObject nwObject)) return;

        nwObject.GetComponent<Pickup_Interactable>().OnPickedUp += CannisterRemoved;
        cannister = nwObject.gameObject;

        nwObject.GetComponent<RigidbodyNetworkTransform>().SetRigidbodyEnabled(false);

        if (!IsServer) return;

        Filled.Value = true;
        nwObject.GetComponent<RigidbodyNetworkTransform>().Teleport(transform.position, transform.rotation, nwObject.transform.lossyScale);
    }

    public void CannisterRemoved()
    {
        Pickup_Interactable pickup_Interactable = cannister.GetComponent<Pickup_Interactable>();
        pickup_Interactable.OnPickedUp -= CannisterRemoved;
        OnGasCannisterRemoved.Invoke();

        cannister.GetComponent<RigidbodyNetworkTransform>().SetRigidbodyEnabled(true);
        cannister = null;

        if (!IsServer) return;

        Filled.Value = false;
    }

    public void SetGasCannisterType(Stock_Item item)
    {
        cannister.GetComponent<StockItem>().SetItem(item);
    }
}
