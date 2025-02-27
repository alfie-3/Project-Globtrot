using Unity.Netcode;
using UnityEngine;

public class PricegunHandler : NetworkBehaviour, IUsePrimary, IUseSecondary
{
    public void UsePrimary(PlayerHoldingManager manager)
    {
        throw new System.NotImplementedException();
    }

    public void UseSecondary(PlayerHoldingManager manager)
    {
        throw new System.NotImplementedException();
    }
}
