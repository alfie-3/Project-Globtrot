using UnityEngine;

public class ItemGrinder : ItemDestructionZone
{
    protected override void DestroyItem(GameObject other)
    {
        if (other.TryGetComponent(out GasCanister gasCanister))
        {
            if (gasCanister.CurrentGasType != GasType.None)
                gasCanister.TriggerExplode_Rpc();
        }

        base.DestroyItem(other);
    }
}
