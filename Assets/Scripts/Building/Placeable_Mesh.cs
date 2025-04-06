using UnityEngine;

public class Placeable_Mesh : MonoBehaviour
{
    [field: SerializeField] public Mesh BuildHologramMesh { get; private set; }

    [field: SerializeField] public bool MirrorX { get; private set; }
    [field: SerializeField] public bool MirrorZ { get; private set; }

    public Matrix4x4 GetMatrix(Vector3 position, Quaternion rotation)
    {
        Vector3 scale = Vector3.one;
        if (MirrorX) scale.x *= -1;
        if (MirrorZ) scale.z *= -1;

        return Matrix4x4.TRS(position, rotation, scale);    
    }

    public bool InvertMaterial()
    {
        if (MirrorX) return true;
        if (MirrorZ) return true;

        return false;
    }
}
