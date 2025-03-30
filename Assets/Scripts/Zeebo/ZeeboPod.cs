using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ZeeboPod : NetworkBehaviour
{
    [SerializeField] List<Zeebo> zeeboPrefabs;
    [SerializeField] float breakForce = 15;
    [SerializeField] float breakoutForce = 3;

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;

        if (collision.impulse.magnitude > breakForce)
        {
            Break(collision);
        }
    }

    public void Break(Collision collision)
    {
        NetworkObject newZeebo = Instantiate(zeeboPrefabs[Random.Range(0, zeeboPrefabs.Count)], transform.position, transform.rotation).NetworkObject;
        newZeebo.Spawn();
        NetworkObject.Despawn();

        newZeebo.GetComponent<Rigidbody>().AddForce(collision.contacts[0].normal * breakoutForce, ForceMode.Impulse);
    }
}
