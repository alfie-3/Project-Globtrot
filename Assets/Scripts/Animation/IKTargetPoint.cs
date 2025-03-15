using System.Data;
using UnityEngine;

public class IKTargetPoint : MonoBehaviour
{
    [SerializeField] private IKTargetsManager.IKCONSTRAINT constraint;

    public void SetConstraint(PlayerHoldingManager manager)
    {
        manager.GetComponentInChildren<IKTargetsManager>().BindTargetToAnchor(constraint, transform, 1);
    }

    public void RemoveConstraint(PlayerHoldingManager manager)
    {
        manager.GetComponentInChildren<IKTargetsManager>().BindTargetToAnchor(constraint, null, 0);
    }
}
