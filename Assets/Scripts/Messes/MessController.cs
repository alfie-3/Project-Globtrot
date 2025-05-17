using Unity.Netcode;
using UnityEngine;

public class MessController : NetworkBehaviour
{
    [field: SerializeField] int sweepsRequired = 3;
    public NetworkVariable<int> CurrentSweepState = new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);
    public bool Cleaned { get; private set; }

    [SerializeField] ParticleSystem SweepParticle;
    [Space]
    public bool Decal;

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

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.root.TryGetComponent(out PlayerCharacterController playerController))
        {
            if (playerController.ImmuneToSlipping) return;
            if (!playerController.IsOwner) return;
            if (!playerController.CharacterMovement.IsGrounded) return;
            if (playerController.PlayerInputManager.MovementInput.magnitude < 0.1f) return;
            if (playerController.RagdollEnabled) return;

            float randomValue = Random.value;

            if (randomValue > (playerController.IsSprinting ? playerController.SprintingSlipChance : playerController.WalkingSlipChance)) return;

            playerController.SetRagdoll(true);
            playerController.Knockout(3);

            PlayerSlip_Rpc(playerController.NetworkObject);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void PlayerSlip_Rpc(NetworkObjectReference player)
    {
        if (!player.TryGet(out NetworkObject other)) return;

        if (other.transform.root.TryGetComponent(out PlayerCharacterController playerController))
        {
            BoxCollider boxCollider = GetComponent<BoxCollider>();

            Collider[] rigidbodyColliders = Physics.OverlapBox(boxCollider.bounds.center, boxCollider.size, transform.rotation, LayerMask.GetMask("PlayerCollider"));

            foreach (Collider playerCollider in playerController.GetComponentsInChildren<Collider>())
            {
                if (playerCollider.attachedRigidbody == null) continue;

                playerCollider.attachedRigidbody.AddExplosionForce(1800, playerController.PlayerModel.Head.transform.position + playerController.PlayerModel.Head.transform.forward, 5);
            }

            playerController.CharacterMovement.Push(playerController.PlayerModel.transform.forward * 200 / 8);
        }
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
        Cleaned = true;
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
        if (current == sweepsRequired) return;

        if (SweepParticle != null)
        {
            SweepParticle.Emit(25);
        }
    }
}
