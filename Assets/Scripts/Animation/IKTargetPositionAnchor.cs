using UnityEngine;

public class IKTargetPositionAnchor : MonoBehaviour
{
    public Transform Anchor;

    public void SetAnchor(Transform anchor)
    {
        Anchor = anchor;
    }

    private void Update()
    {
        if (Anchor == null) { return; }

        transform.SetPositionAndRotation(Anchor.position, Anchor.rotation);
    }
}
