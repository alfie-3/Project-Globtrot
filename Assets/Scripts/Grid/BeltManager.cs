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



    [field: SerializeField] private List<Belt> Beltss;
     

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else
            Destroy(this);
    }

    public static void AddMe(Belt body, ref Action<Belt> destroy) {
        Instance.Beltss.Add(body);
        destroy += (Belt b) => {Instance.Beltss.Remove(b);};
    }


    private void FixedUpdate() {
        foreach (Belt b in Beltss) {
            b.Jiggle(Speed);
        }
    }
}
