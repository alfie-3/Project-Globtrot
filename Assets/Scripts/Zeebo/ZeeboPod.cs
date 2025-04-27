using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ZeeboPod : NetworkBehaviour
{
    [SerializeField] List<Zeebo> zeeboPrefabs;

    [SerializeField] float breakoutForce = 3;

    public void Break(PhysicsObjectHealth health)
    {
        NetworkObject newZeebo = Instantiate(zeeboPrefabs[Random.Range(0, zeeboPrefabs.Count)], transform.position, transform.rotation).NetworkObject;
        newZeebo.Spawn();

        newZeebo.GetComponent<Rigidbody>().AddForce(health.LastHitNormal * breakoutForce, ForceMode.Impulse);
    }
}
