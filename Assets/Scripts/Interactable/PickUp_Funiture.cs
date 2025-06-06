using Unity.Netcode;
using UnityEngine;
using System.Collections;
using Unity.Collections;
using Assets.Scripts.Interfaces;

public class PickUp_Funiture : Pickup_Interactable,  IDismantleable// : Pickup_Interactable {
{
    const string CrateID = "crate";
    public PlacableFurniture_Item placableFurniture;

    public override void OnInteract(PlayerInteractionManager interactionManager) { }
    public void OnDismantle(PlayerInteractionManager interactionManager) {

        if (interactionManager.TryGetComponent(out PlayerHoldingManager holdingManager))
        {
            if (holdingManager.HoldingItem) return;
        }
        else return;

        GiveCrate_RPC(placableFurniture.ItemID, interactionManager.NetworkObject);
        RequestRemove_RPC();
    }

    [Rpc(SendTo.Server)]
    private void GiveCrate_RPC(string itemID, NetworkObjectReference holderReference)
    {
        if (!holderReference.TryGet(out NetworkObject obj))
            return;

        PlacableFurniture_Item placeableItem = ItemDictionaryManager.RetrieveItem(CrateID) is not PlacableFurniture_Item ? null : (PlacableFurniture_Item)ItemDictionaryManager.RetrieveItem(CrateID);

        if (placeableItem == null) return;
        NetworkObject instance = Instantiate(placeableItem.FurniturePrefab, obj.transform.position, transform.rotation).GetComponent<NetworkObject>();
        instance.Spawn();

        instance.GetComponent<FurnitureBoxController>().SetItem_Rpc(itemID, transform.rotation.eulerAngles.y);

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
