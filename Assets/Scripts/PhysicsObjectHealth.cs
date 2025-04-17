using System;
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

    private void Awake()
    {
        health.OnValueChanged += OnUpdateHealth;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        health.Value = baseHealth;
    }

    private void OnUpdateHealth(float previousValue, float newValue)
    {
        if (newValue < previousValue && newValue > 0)
        {
            onTakeDamage.Invoke();
            return;
        }

        if (newValue <= 0)
        {
            onDeath.Invoke();
            return;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!ShouldTakeDamage) return;
        if (!IsServer) return;

        if (collision.relativeVelocity.magnitude > minDamageForce)
        {
            float damageMultiplier = damageCurve.Evaluate(collision.relativeVelocity.magnitude / maxDamageForce) + 1;

            health.Value -= fragility * damageMultiplier;
        }
    }
}
