using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Beltuy : NetworkBehaviour
{
    public bool hasItem;
    public float speed = 4f;
    public Vector3 velocity;
    private void Awake() {
        objs = new();
    }
    private void Update() {
        /*foreach (NetworkObject obj in objs) {
            obj.GetComponent<RigidbodyNetworkTransform>().AddForce_Rpc(transform.forward * speed, ForceMode.Acceleration);
        }*/
    }

    public List<NetworkObject> objs;
    private void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.TryGetComponent<NetworkObject>(out NetworkObject obj)) objs.Add(obj);
        Debug.Log(collision.gameObject.name + " entered");
        hasItem = true;
    }
    private void OnCollisionExit(Collision collision) {
        if (collision.gameObject.TryGetComponent<NetworkObject>(out NetworkObject obj)) objs.Remove(obj);
        Debug.Log(collision.gameObject.name + " left");
        hasItem = false;
    }
    private void OnCollisionStay(Collision collision) {
        collision.gameObject.GetComponent<RigidbodyNetworkTransform>().AddForce_Rpc(transform.forward * speed, ForceMode.Acceleration);
        velocity = collision.gameObject.GetComponent<Rigidbody>().linearVelocity;
    }
}
