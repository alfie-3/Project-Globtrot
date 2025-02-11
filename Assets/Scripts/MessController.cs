using Unity.Netcode;
using UnityEngine;

public class MessController : NetworkBehaviour
{
    [SerializeField] int sweepsRequired = 3;

    public NetworkVariable<int> CurrentSweepState = new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer) return;

        CurrentSweepState.Value = sweepsRequired;
    }

    [Rpc(SendTo.Server)]
    public void RequestSweep_Rpc(int sweepPower)
    {
        Sweep(sweepPower);
    }

    private void Sweep(int sweepPower)
    {
        CurrentSweepState.Value -= sweepPower;

        if (CurrentSweepState.Value <= 0)
            Clean();
    }

    private void Clean()
    {
        NetworkObject.Despawn(true);
    }
}
