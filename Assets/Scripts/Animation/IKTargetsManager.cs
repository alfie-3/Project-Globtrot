using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class IKTargetsManager : NetworkBehaviour
{
    public TwoBoneIKConstraint LeftHandIK;
    public TwoBoneIKConstraint RightHandIK;

    public enum IKCONSTRAINT
    {
        LeftHand,
        RightHand
    }

    public void BindTargetToAnchor(IKCONSTRAINT constraint, Transform target, float weight = 1)
    {
        switch (constraint)
        {
            case IKCONSTRAINT.LeftHand:
                LeftHandIK.data.target.GetComponent<IKTargetPositionAnchor>().SetAnchor(target);
                LeftHandIK.weight = weight;
                break;
            case IKCONSTRAINT.RightHand:
                RightHandIK.data.target.GetComponent<IKTargetPositionAnchor>().SetAnchor(target);
                RightHandIK.weight = weight;
                break;
        }
    }

    public void ConstrainIKToObject(GameObject gameObject)
    {
        foreach (IKTargetPoint targetPoint in gameObject.GetComponentsInChildren<IKTargetPoint>())
        {
            targetPoint.SetConstraint(GetComponentInChildren<IKTargetsManager>());
        }
    }

    public void ClearIKToObject(GameObject gameObject)
    {
        foreach (IKTargetPoint targetPoint in gameObject.GetComponentsInChildren<IKTargetPoint>())
        {
            targetPoint.RemoveConstraint(GetComponentInChildren<IKTargetsManager>());
        }
    }

    [Rpc(SendTo.Everyone)]
    public void ClearAllIKTargets_Rpc()
    {
        BindTargetToAnchor(IKCONSTRAINT.LeftHand, null, 0);
        BindTargetToAnchor(IKCONSTRAINT.RightHand, null, 0);
    }
}
