using Unity.Netcode;
using UnityEngine;

public class MessController : NetworkBehaviour
{
    [SerializeField] int sweepsRequired = 3;
    public NetworkVariable<int> CurrentSweepState = new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

    [SerializeField] ParticleSystem SweepParticle;

    private void OnEnable()
    {
        CurrentSweepState.OnValueChanged += PlayEffect;
    }

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
        if (SweepParticle != null)
        {
            SweepParticle.transform.parent = null;

            var main = SweepParticle.main;
            main.stopAction = ParticleSystemStopAction.Destroy;
        }

        NetworkObject.Despawn(true);
    }

    public void PlayEffect(int prev, int current)
    {
        if (SweepParticle != null)
        {
            SweepParticle.Emit(25);
        }
    }
}
