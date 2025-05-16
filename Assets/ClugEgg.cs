using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ClugEgg : NetworkBehaviour
{
    [SerializeField] NetworkObject eggMess;
    [SerializeField] GameObject particleEffect;

    [SerializeField] float breakoutForce = 3;

    public void Break(PhysicsObjectHealth health)
    {
        if (IsServer)
        {
            NetworkObject newEggMess = Instantiate(eggMess, transform.position, transform.rotation);
            newEggMess.Spawn();

            newEggMess.GetComponent<Rigidbody>().AddForce(health.LastHitNormal * breakoutForce, ForceMode.Impulse);
        }

        GameObject effect = Instantiate(particleEffect, transform.position, transform.rotation);
        effect.transform.localScale *= 0.8f;
    }
}
