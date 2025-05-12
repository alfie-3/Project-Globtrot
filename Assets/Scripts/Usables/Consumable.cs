using Unity.Netcode;
using UnityEngine;

public class Consumable : NetworkBehaviour, IUsePrimary
{
    public PlayerStatModifier ConsumableModifier = PlayerStatModifier.MovementSpeed;
    [SerializeField] float multiplierAmount = 2f;
    [SerializeField] float bathroomNeedMultiplier = 2f;

    bool consumed = false;

    public void UsePrimary(PlayerHoldingManager manager)
    {
        Consume(manager);
    }

    public void Consume(PlayerHoldingManager manager)
    {
        if (consumed) return;

        if (manager.TryGetComponent(out PlayerStatManager statManager))
        {
            statManager.UpdateState(ConsumableModifier, multiplierAmount);
            statManager.UpdateState(PlayerStatModifier.BathroomNeed, bathroomNeedMultiplier);
        }

        consumed = true;
        Consumed_Rpc();
    }

    [Rpc(SendTo.Everyone)]
    public void Consumed_Rpc()
    {
        if (IsServer)
            NetworkObject.Despawn();
    }
}