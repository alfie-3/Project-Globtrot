using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System;

public class BeltManager : NetworkBehaviour
{
    public static BeltManager Instance { get; private set; }

    [field: SerializeField] public float Speed { get; private set; }
    [field: SerializeField] public float PlayerForce { get; private set; }
    public LayerMask PlayerLayer => LayerMask.GetMask("Player");


    //NetworkList<Rigidbody> Belts = new NetworkList<Rigidbody>(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

    [field: SerializeField] private List<Rigidbody> Beltss;
     

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else
            Destroy(this);
    }

    public static void AddMe(Rigidbody body, Action<Rigidbody> destroy) {
        Instance.Beltss.Add(body);
        destroy += (Rigidbody b) => {Instance.Beltss.Remove(b);};
        
    }


    private void FixedUpdate() {
        foreach (Rigidbody b in Beltss) {
            Vector3 pos = b.position;
            b.position -= transform.forward * Speed * Time.fixedDeltaTime;
            b.MovePosition(pos);
        }


        
    }
}
