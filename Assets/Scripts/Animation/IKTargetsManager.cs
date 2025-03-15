using UnityEngine;
using UnityEngine.Animations.Rigging;

public class IKTargetsManager : MonoBehaviour
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
        switch(constraint)
        {
            case IKCONSTRAINT.LeftHand:
                LeftHandIK.data.target.GetComponent<TargetPositionAnchor>().SetAnchor(target);
                LeftHandIK.weight = weight;
                break;
            case IKCONSTRAINT.RightHand:
                RightHandIK.data.target.GetComponent<TargetPositionAnchor>().SetAnchor(target);
                RightHandIK.weight = weight;
                break;
        }
    }
}
