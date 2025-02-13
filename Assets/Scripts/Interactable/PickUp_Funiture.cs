using Unity.Netcode;
using UnityEngine;

public class PickUp_Funiture : Pickup_Interactable {
    public string itemID;
    public override void OnInteract(PlayerInteractionManager interactionManager) {
        //Pickup(interactionManager);
        GiveCrate_RPC(itemID,interactionManager.NetworkObject);
        RequestRemove_RPC();
    }

    [Rpc(SendTo.Server)]
    private void GiveCrate_RPC(string itemID, NetworkObjectReference holderReference)
    {
        if (!holderReference.TryGet(out NetworkObject obj))
            return;
        PlacableFurniture_Item placeableItem = ItemDictionaryManager.RetrieveItem("Crate") is not PlacableFurniture_Item ? null : (PlacableFurniture_Item)ItemDictionaryManager.RetrieveItem("Crate");
        if (placeableItem == null) return;
        NetworkObject instance = Instantiate(placeableItem.FurniturePrefab).GetComponent<NetworkObject>();
        instance.Spawn();
        instance.GetComponent<FurnitureBoxController>().SetItem(itemID);

        RpcSendParams sendParams = new RpcSendParams()
        {
            Target = RpcTarget.Single(obj.OwnerClientId, RpcTargetUse.Temp)
        };

        RpcParams rpcParams = new RpcParams()
        {
            Send = sendParams
        };

        GiveHolderCrate_Rpc(instance, holderReference, rpcParams);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void GiveHolderCrate_Rpc(NetworkObjectReference newInstanceRef, NetworkObjectReference holderReference, RpcParams rpcParams)
    {
        if (!holderReference.TryGet(out NetworkObject holder))
            return;

        if (!newInstanceRef.TryGet(out NetworkObject newInstance))
        {
            Debug.Log("NewInstanceNull");
            return;
        }

        newInstance.GetComponent<Pickup_Interactable>().OnInteract(holder.GetComponent<PlayerInteractionManager>());
    }
}
