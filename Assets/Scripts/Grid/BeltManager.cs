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

    [field: SerializeField] protected Material BeltMaterial { get; private set; }

    private float shaderRatio;
    private float playerRatio;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else
            Destroy(this);

    }

    public static void AddMe(Belt body) {
        Instance.Beltss.Add(body);
        body.OnDestroyAction += (Belt b) => Instance.Beltss.Remove(b);
        //destroy += (Belt b) => {Instance.Beltss.Remove(b);};
    }


    private void FixedUpdate() {
        Beltss.ForEach(belt => belt.Jiggle(Speed));
    }


    public void SetBeltSpeed(float speed) {
        float mult = speed / Speed;
        Speed = speed;
        PlayerForce *= mult;
        BeltMaterial.SetFloat("_Speed", BeltMaterial.GetFloat("_Speed")*mult);

        //Beltss[0](belt => belt.GetComponent<Renderer>().sharedMaterial.SetFloat("_Speed", belt.GetComponent<Renderer>().sharedMaterial.GetFloat("_Speed") *mult));
    }

    private void OnDestroy() {
        BeltMaterial.SetFloat("_Speed", 0.68f);
    }
}
