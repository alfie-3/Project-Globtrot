using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class FurnitureBoxController : NetworkBehaviour, IUsePrimary, IUpdate
{
    [SerializeField]
    private PlacableFurniture_Item furnitureItem;

    [SerializeField] private Material ableToBePlaced;
    [SerializeField] private Material notAbleToBePlaced;

    private Mesh holoMesh;
    private RenderParams renderParams;

    public const float PLACABLE_DISTANCE = 5;

    private void Awake() {
        renderParams = new RenderParams(ableToBePlaced);
    }

    [Rpc(SendTo.Everyone)]
    public void SetItem_Rpc(string itemID)
    {
        furnitureItem = ItemDictionaryManager.RetrieveItem(itemID) is not PlacableFurniture_Item ? null : (PlacableFurniture_Item)ItemDictionaryManager.RetrieveItem(itemID);
        if (furnitureItem.FurniturePrefab.TryGetComponent(out MeshFilter meshFilter)) {
            //Graphics.RenderMesh(rp, meshFilter.sharedMesh, 0, Matrix4x4.TRS((hit.point + furnitureItem.FurniturePrefab.transform.position), Quaternion.Euler(0, holdingManager.Rotation, 0), Vector3.one));
            holoMesh = meshFilter.sharedMesh;
            
        }
        //holoMesh = 
    }

    public void UsePrimary(PlayerHoldingManager holdingManager)
    {
        if (furnitureItem == null) return;

        Ray ray = new(holdingManager.CameraManager.CamTransform.position, holdingManager.CameraManager.CamTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, PLACABLE_DISTANCE, LayerMask.GetMask("Placeable")) && Physics.OverlapBox(hit.point + holoMesh.bounds.center, holoMesh.bounds.size * 0.48f).Length == 0)
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
            renderParams.material = Physics.OverlapBox(hit.point + holoMesh.bounds.center, holoMesh.bounds.size*0.48f).Length == 0 ? ableToBePlaced : notAbleToBePlaced;
            if (furnitureItem.FurniturePrefab.TryGetComponent(out MeshFilter meshFilter))
            {
                Graphics.RenderMesh(renderParams, meshFilter.sharedMesh, 0, Matrix4x4.TRS((hit.point + furnitureItem.FurniturePrefab.transform.position), Quaternion.Euler(0, holdingManager.Rotation, 0), Vector3.one));
            }
        }
    }
    void OnDrawGizmosSelected() {
        // Draw a yellow cube at the transform position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position+holoMesh.bounds.center, holoMesh.bounds.max);
    }
}
