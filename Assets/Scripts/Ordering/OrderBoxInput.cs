using Unity.Netcode;
using UnityEngine;

public class OrderBoxInput : NetworkBehaviour
{
    [SerializeField] ParticleSystem inputParticle;
    [Space]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClipRandomData randomClip;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if (!other.TryGetComponent(out NetworkObject networkObject)) return;

        if (other.TryGetComponent(out StockItem stockItem))
        {
            if (stockItem.Item == null) return;

            if (!transform.parent.TryGetComponent(out ContainerContents contents)) return;

            if (!contents.Contents.TryAddItem(stockItem.Item, 1)) return;

            ClipData clipData = randomClip.GetClip(1, true);

            audioSource.pitch = clipData.Pitch;
            audioSource.PlayOneShot(clipData.Clip);

            networkObject.Despawn();

            if (inputParticle != null)
            {
                PlayParticle_Rpc();
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    public void PlayParticle_Rpc()
    {
        inputParticle.Play();
    }
}
