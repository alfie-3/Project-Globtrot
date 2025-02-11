using UnityEngine;

[CreateAssetMenu(fileName = "New Mop Item", menuName = "Items/Mop")]
public class Mop_Item : ItemBase
{
    [SerializeField] float sweepDistance = 5f;
    [SerializeField] int sweepPower = 1;

    public override void OnHeld()
    {
        base.OnHeld();
        Debug.Log("Mop Equiped");
    }

    public override void OnPrimary(PlayerHoldingManager holdingManager)
    {
        base.OnPrimary(holdingManager);

        Sweep(holdingManager);
    }

    public void Sweep(PlayerHoldingManager holdingManager)
    {
        Ray ray = new(holdingManager.CameraManager.CamTransform.position, holdingManager.CameraManager.CamTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, sweepDistance))
        {
            if (hit.collider.TryGetComponent(out MessController messController))
            {
                messController.RequestSweep_Rpc(sweepPower);
            }

        }
    }
}
