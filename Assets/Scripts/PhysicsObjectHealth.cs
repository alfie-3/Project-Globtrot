using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class PhysicsObjectHealth : NetworkBehaviour
{
    [SerializeField] UnityEvent onTakeDamage;
    [SerializeField] UnityEvent onDeath;
    [Space]
    public bool ShouldTakeDamage;
    [SerializeField] float baseHealth;
    [SerializeField] NetworkVariable<float> health = new();
    [Space]
    [SerializeField] float minDamageForce;
    [SerializeField] float maxDamageForce;
    [SerializeField] AnimationCurve damageCurve;
    [Space]
    [SerializeField] float fragility;

    bool killed = false;
    bool dead = false;

    public Vector3 LastHitNormal { get; private set; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        health.OnValueChanged += OnUpdateHealth;

        if (!IsServer) return;

        health.Value = baseHealth;
    }

    private void OnUpdateHealth(float previousValue, float newValue)
    {
        if (newValue <= 0 && IsServer)
        {
            Kill_Rpc();

            if (NetworkObject.IsSpawned)
            {
                Kill_Rpc();
                NetworkObject.Despawn();
            }
        }

        if (newValue < previousValue)
        {
            onTakeDamage.Invoke();
            return;
        }
    }

    [Rpc(SendTo.Everyone)]
    public void Kill_Rpc()
    {
        killed = true;
    }

    public override void OnNetworkDespawn()
    {
        if (!killed) return;
        if (dead) return;
        dead = true;
        onDeath.Invoke();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!ShouldTakeDamage) return;
        if (!IsServer) return;

        if (collision.relativeVelocity.magnitude > minDamageForce)
        {
            float damageMultiplier = damageCurve.Evaluate(collision.relativeVelocity.magnitude / maxDamageForce) + 1;
            LastHitNormal = collision.contacts[0].normal;

            health.Value -= fragility * damageMultiplier;
        }
    }
}
