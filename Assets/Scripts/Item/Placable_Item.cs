using Unity.AppUI.UI;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "New Placable Item", menuName = "Items/Placable")]
public class Placable_Item : ItemBase
{
    [SerializeField] GameObject placeablePrefab;

    const float PLACABLE_DISTANCE = 5;

    public override void OnPrimary(PlayerHoldingManager holdingManager)
    {
        base.OnPrimary(holdingManager);

        if (placeablePrefab != null)
            PlaceItem(holdingManager);
    }

    public void PlaceItem(PlayerHoldingManager holdingManager)
    {
        Ray ray = new(holdingManager.CameraManager.CamTransform.position, holdingManager.CameraManager.CamTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, PLACABLE_DISTANCE, LayerMask.GetMask("Placeable")))
        {
            NetworkObject instance = Instantiate(placeablePrefab, hit.point + placeablePrefab.transform.position, Quaternion.identity).GetComponent<NetworkObject>();
            instance.Spawn();

            holdingManager.ClearItem();
        }
    }
}
