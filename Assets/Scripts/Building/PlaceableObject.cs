using Unity.Netcode;
using UnityEngine;

public class PlaceableObject : NetworkBehaviour
{
    [field: SerializeField] public Mesh BuildHologramMesh { get; private set; }
    [field: SerializeField] public PlacableFurniture_Item item { get; private set; }
    [field: SerializeField] public bool CanRemove { get; private set; } = true;
    [field: Space]
    [field: SerializeField] public bool MirrorX { get; private set; }
    [field: SerializeField] public bool MirrorZ { get; private set; }

    public override void OnNetworkSpawn()
    {
        RequiredBuildablesManager.AddBuildable(item);
    }

    public override void OnNetworkDespawn()
    {
        RequiredBuildablesManager.RemoveBuildable(item);
    }

    public Matrix4x4 GetMatrix(Vector3 position, Quaternion rotation)
    {
        Vector3 scale = Vector3.one;
        if (MirrorX) scale.x *= -1;
        if (MirrorZ) scale.z *= -1;

        return Matrix4x4.TRS(position, rotation, scale);    
    }

    public bool InvertMaterial()
    {
        return MirrorX || MirrorZ;
    }
}
