using Unity.Netcode;
using UnityEngine;

public class FurnitureBoxController : NetworkBehaviour, IUsePrimary, IUpdate
{
    [SerializeField]
    private PlacableFurniture_Item furnitureItem;

    private Mesh holoMesh;
    private RenderParams renderParams;

    public const float PLACABLE_DISTANCE = 5;

    public void SetItem(string itemID)
    {
        furnitureItem = ItemDictionaryManager.RetrieveItem(itemID) is not PlacableFurniture_Item ? null : (PlacableFurniture_Item)ItemDictionaryManager.RetrieveItem(itemID);
    }

    public void UsePrimary(PlayerHoldingManager holdingManager)
    {
        if (furnitureItem == null) return;

        Ray ray = new(holdingManager.CameraManager.CamTransform.position, holdingManager.CameraManager.CamTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, PLACABLE_DISTANCE, LayerMask.GetMask("Placeable")))
        {
            holdingManager.PlaceItem_Rpc(NetworkObject, furnitureItem.ItemID, hit.point, Quaternion.Euler(0, holdingManager.Rotation, 0));
        }
    }

    public void OnUpdate(PlayerHoldingManager holdingManager)
    {
        if (furnitureItem == null) return;
        if (!holdingManager.IsLocalPlayer) return;

        Ray ray = new(holdingManager.CameraManager.CamTransform.position, holdingManager.CameraManager.CamTransform.forward);
        RenderParams rp = new RenderParams(holdingManager.Material);
        if (Physics.Raycast(ray, out RaycastHit hit, PLACABLE_DISTANCE, LayerMask.GetMask("Placeable")))
        {
            if (furnitureItem.FurniturePrefab.TryGetComponent(out MeshFilter meshFilter))
            {
                Graphics.RenderMesh(rp, meshFilter.sharedMesh, 0, Matrix4x4.TRS((hit.point + furnitureItem.FurniturePrefab.transform.position), Quaternion.Euler(0, holdingManager.Rotation, 0), Vector3.one));
            }
        }
    }
}
