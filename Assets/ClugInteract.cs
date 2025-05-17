using Unity.Netcode;
using UnityEngine;

public class ClugInteract : NetworkBehaviour, IUsePrimary
{

    [SerializeField] Animator clugAnimator;
    [SerializeField] AudioSource audioSrc;

    [SerializeField] Transform eggSpawnPos;
    [SerializeField] GameObject egg;

    [SerializeField] float eggSpawnCooldown = 1f;
    public void UsePrimary(PlayerHoldingManager manager)
    {
        Squeeze_Rpc();
    }

    [Rpc(SendTo.Everyone)]
    public void Squeeze_Rpc()
    {
        //clugAnimator.Play("Clug_Squeeze");
        clugAnimator.CrossFade("Clug_Squeeze", 0.25f);
        audioSrc.pitch = Random.Range(0.9f, 1.1f);
        audioSrc.Play();

        if (!IsServer) return;

        GameObject spawnedEgg = Instantiate(egg, eggSpawnPos.position, Quaternion.identity);
        spawnedEgg.GetComponent<NetworkObject>().Spawn();
    }
}
