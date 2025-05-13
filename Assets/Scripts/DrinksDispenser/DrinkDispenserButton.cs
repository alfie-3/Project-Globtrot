using Unity.Netcode;
using UnityEngine;

public class DrinkDispenserButton : NetworkBehaviour, IInteractable
{
    [SerializeField] DrinksDispenser drinksDispenser;
    [SerializeField] NetworkObject drinkDispensePrefab;

    public void OnInteract(PlayerInteractionManager interactionManager)
    {
        Dispense_Rpc();
    }

    [Rpc(SendTo.Everyone)]
    public void Dispense_Rpc()
    {
        drinksDispenser.Dispense(drinkDispensePrefab);
    }
}
