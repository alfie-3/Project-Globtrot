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



    [field: SerializeField] private List<Belt2> Beltss;
     

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else
            Destroy(this);
    }

    public static void AddMe(Belt2 body, ref Action<Belt2> destroy) {
        Instance.Beltss.Add(body);
        destroy += (Belt2 b) => {Instance.Beltss.Remove(b);};
    }


    private void FixedUpdate() {
        foreach (Belt2 b in Beltss) {
            b.Jiggle(Speed);
        }
    }
}
