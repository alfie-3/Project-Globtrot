using Unity.Netcode;
using UnityEngine;

public class ClugInteract : NetworkBehaviour, IUsePrimary
{

    [SerializeField] Animator clugAnimator;
    [SerializeField] AudioSource audioSrc;

    [SerializeField] Transform eggSpawnPos;
    [SerializeField] GameObject egg;

    [SerializeField] float eggSpawnCooldown = 1f;
    NetworkVariable<float> eggSpawnTimer = new();

    public void UsePrimary(PlayerHoldingManager manager)
    {
        Squeeze_Rpc();
    }

    private void Update()
    {
        if (!IsServer) return;

        if (eggSpawnTimer.Value > 0)
        {
            eggSpawnTimer.Value -= Time.deltaTime;
        }
    }

    public void UpdateTimer()
    {
        if (eggSpawnTimer.Value > 0)
        {
            eggSpawnTimer.Value -= Time.deltaTime;
        }
    }

    [Rpc(SendTo.Everyone)]
    public void Squeeze_Rpc()
    {
        if (eggSpawnTimer.Value > 0) return;

        //clugAnimator.Play("Clug_Squeeze");
        clugAnimator.CrossFade("Clug_Squeeze", 0.25f);
        audioSrc.pitch = Random.Range(0.9f, 1.1f);
        audioSrc.Play();

        if (!IsServer) return;

        eggSpawnTimer.Value = eggSpawnCooldown;

        GameObject spawnedEgg = Instantiate(egg, eggSpawnPos.position, Quaternion.identity);
        spawnedEgg.GetComponent<NetworkObject>().Spawn();
    }

    public InteractionContext GetUseContext(PlayerHoldingManager holdingManager)
    {
        return new(true, "Squeeze");
    }
}
