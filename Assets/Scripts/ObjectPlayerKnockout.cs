using System;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObjectPlayerKnockout : MonoBehaviour
{
    [SerializeField] float minRequiredVelocity = 7;
    float explosiveForce = 2000;
    [Space]
    [SerializeField] bool impactEnabed = false;


    private void OnEnable()
    {
        if (TryGetComponent(out Pickup_Interactable pickInt))
        {
            pickInt.OnDropped += EnableImpact;
            pickInt.OnPickedUp += DisableImpact;
        }

        GetComponent<RigidbodyNetworkTransform>().OnSleepingChanged += UpdateImpactEnabed;
    }

    private void DisableImpact()
    {
        impactEnabed = false;
    }

    private void OnDisable()
    {
        GetComponent<RigidbodyNetworkTransform>().OnSleepingChanged -= UpdateImpactEnabed;

    }

    private void UpdateImpactEnabed(bool obj)
    {
        if (obj == true)
        {
            impactEnabed = false;
        }
    }

    public void EnableImpact()
    {
        Invoke(nameof(SetImpactEnabled), 0.1f);
    }

    public void SetImpactEnabled()
    {
        impactEnabed = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!impactEnabed) return;
        if (collision.relativeVelocity.magnitude < minRequiredVelocity) return;

        Debug.Log(collision.relativeVelocity.magnitude);

        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerCollider"))
        {
            if (!collision.transform.root.TryGetComponent(out PlayerCharacterController playerCharacterController)) return;

            playerCharacterController.SetRagdoll(true);
            playerCharacterController.Knockout(1.5f);


            ProcessImpact_Rpc(collision.contacts[0].point);
        }
    }

    [Rpc(SendTo.Server)]
    public void ProcessImpact_Rpc(Vector3 position)
    {

        Collider[] colliders = Physics.OverlapSphere(position, 2, LayerMask.GetMask("PlayerCollider"));

        foreach (Collider collider in colliders)
        {
            if (!collider.attachedRigidbody) return;
            collider.attachedRigidbody.AddExplosionForce(explosiveForce, position, 2f);
        }
    }
}
