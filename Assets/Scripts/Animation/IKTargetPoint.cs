using System.Data;
using UnityEngine;

public class IKTargetPoint : MonoBehaviour
{
    [SerializeField] private IKTargetsManager.IKCONSTRAINT constraint;

    public void SetConstraint(IKTargetsManager manager)
    {
        manager.BindTargetToAnchor(constraint, transform, 1);
    }

    public void RemoveConstraint(IKTargetsManager manager)
    {
        manager.BindTargetToAnchor(constraint, null, 0);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}

