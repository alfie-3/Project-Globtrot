using Unity.Netcode;
using UnityEngine;

public class PickUp_Funiture : Pickup_Interactable {
    public override void OnInteract(PlayerInteractionManager interactionManager) {
        Pickup(interactionManager);
        RequestRemove_RPC();
    }
}
