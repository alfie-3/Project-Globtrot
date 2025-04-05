using Unity.Netcode;
using UnityEngine;

public class PlayerMultiplayerMaterialsController : NetworkBehaviour
{
    protected override void OnNetworkPostSpawn()
    {
        if (IsLocalPlayer)
        {
            EnableMeshCulling();
        }
    }

    public void EnableMeshCulling()
    {
        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();

        propBlock.SetFloat("_AlphaClipThreshold", 0.5f);

        GetComponent<SkinnedMeshRenderer>().SetPropertyBlock(propBlock);
    }
}
