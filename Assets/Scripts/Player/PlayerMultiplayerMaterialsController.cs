using Unity.Netcode;
using UnityEngine;

public class PlayerMultiplayerMaterialsController : NetworkBehaviour
{
    protected override void OnNetworkPostSpawn()
    {
        if (IsLocalPlayer)
        {
            SetMeshCulling(true);
        }
    }

    public void SetMeshCulling(bool value)
    {
        float threshold = value ? 0.5f : 0;

        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();

        propBlock.SetFloat("_AlphaClipThreshold", threshold);

        SkinnedMeshRenderer meshRenderer = GetComponent<SkinnedMeshRenderer>();

        for (int i = 0; i < meshRenderer.materials.Length; i++)
        {
            meshRenderer.SetPropertyBlock(propBlock, i);
        }
    }
}
