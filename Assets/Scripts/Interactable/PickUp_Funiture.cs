using Unity.Netcode;
using UnityEngine;

public class PickUp_Funiture : Pickup_Interactable {
    public string itemID;
    public override void OnInteract(PlayerInteractionManager interactionManager) {
        //Pickup(interactionManager);
        GiveCrate_RPC(itemID,interactionManager.NetworkObject);
        RequestRemove_RPC();
    }

    
    private void GiveHolderCrate_RPC()
    {

    }
    [Rpc(SendTo.Server)]
    private void GiveCrate_RPC(string itemID, NetworkObjectReference HolderReference)
    {
        if (!HolderReference.TryGet(out NetworkObject obj))
            return;
        PlacableFurniture_Item placeableItem = ItemDictionaryManager.RetrieveItem("Crate") is not PlacableFurniture_Item ? null : (PlacableFurniture_Item)ItemDictionaryManager.RetrieveItem("Crate");
        if (placeableItem == null) return;
        NetworkObject instance = Instantiate(placeableItem.FurniturePrefab).GetComponent<NetworkObject>();
        instance.Spawn();
        instance.GetComponent<FurnitureBoxController>().SetItem(itemID);
        instance.GetComponent<Pickup_Interactable>().OnInteract(obj.GetComponent<PlayerInteractionManager>());
    }
}
