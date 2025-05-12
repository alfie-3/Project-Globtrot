using Unity.Netcode;
using UnityEngine;

public class DrinkDispenserButton : NetworkBehaviour, IInteractable
{
    [SerializeField] Transform drinkDispenseTransform;
    [SerializeField] NetworkObject drinkDispensePrefab;

    public void OnInteract(PlayerInteractionManager interactionManager)
    {
        Dispense_Rpc();
    }

    [Rpc(SendTo.Everyone)]
    public void Dispense_Rpc()
    {
        if (IsServer)
        {
            NetworkObject newDrink = Instantiate(drinkDispensePrefab, drinkDispenseTransform.position, drinkDispenseTransform.rotation);
            newDrink.Spawn();
        }
    }
}
