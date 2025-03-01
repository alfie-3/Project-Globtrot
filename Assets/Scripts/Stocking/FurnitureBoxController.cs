using Unity.Netcode;
using UnityEngine;

public class FurnitureBoxController : NetworkBehaviour, IUsePrimary, IUpdate
{
    [SerializeField]
    private PlacableFurniture_Item furnitureItem;

    [SerializeField] private Material HologramMat;

    private Mesh holoMesh;
    private RenderParams renderParams;

    public const float PLACABLE_DISTANCE = 5;

    GridController grid;
    private void Start() {
        renderParams = new RenderParams(HologramMat);
        
        if (furnitureItem != null)
            PopulateItem(furnitureItem);

        grid = GridController.Instance;
    }

    public void Init(Grid grid)
    {
         
    }

    [Rpc(SendTo.Everyone)]
    public void SetItem_Rpc(string itemID)
    {
        furnitureItem = ItemDictionaryManager.RetrieveItem(itemID) is not PlacableFurniture_Item ? null : (PlacableFurniture_Item)ItemDictionaryManager.RetrieveItem(itemID);
        if (furnitureItem == null) return;

        PopulateItem(furnitureItem);
    }

    public void PopulateItem(PlacableFurniture_Item furnitureItem)
    {
        if (furnitureItem.FurniturePrefab.TryGetComponent(out MeshFilter meshFilter))
        {
            holoMesh = meshFilter.sharedMesh;
        }
    }

    public void UsePrimary(PlayerHoldingManager holdingManager)
    {
        if (furnitureItem == null) return;

        Ray ray = new(holdingManager.CameraManager.CamTransform.position, holdingManager.CameraManager.CamTransform.forward);


        if (Physics.Raycast(ray, out RaycastHit hit, PLACABLE_DISTANCE, LayerMask.GetMask("Placeable")))
        {
            Vector3 position = holdingManager.Snapping ? grid.HitToGrid(hit.point) : hit.point;
            if (Physics.OverlapBox(position + holoMesh.bounds.center, holoMesh.bounds.size * 0.48f, Quaternion.Euler(0, holdingManager.Rotation, 0)).Length == 0)
                holdingManager.PlaceItem_Rpc(NetworkObject, furnitureItem.ItemID, position, Quaternion.Euler(0, holdingManager.Rotation, 0));
        }
    }

    public void OnUpdate(PlayerHoldingManager holdingManager)
    {
        if (furnitureItem == null) return;
        if (!holdingManager.IsLocalPlayer) return;

        Ray ray = new(holdingManager.CameraManager.CamTransform.position, holdingManager.CameraManager.CamTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, PLACABLE_DISTANCE, LayerMask.GetMask("Placeable")))
        {
            Vector3 position = holdingManager.Snapping ? grid.HitToGrid(hit.point) : hit.point;
            HologramMat.SetFloat("_OverlappingColliders", Physics.OverlapBox(position+ holoMesh.bounds.center, holoMesh.bounds.size * 0.48f, Quaternion.Euler(0, holdingManager.Rotation, 0)).Length);
            Graphics.RenderMesh(renderParams, holoMesh, 0, Matrix4x4.TRS(position, Quaternion.Euler(0, holdingManager.Rotation, 0), Vector3.one));
        }
    }



    void OnDrawGizmosSelected() {
        if (holoMesh == null) return;

        // Draw a yellow cube at the transform position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position+holoMesh.bounds.center, holoMesh.bounds.max);
    }
}
