using Unity.Multiplayer.Center.NetcodeForGameObjectsExample.DistributedAuthority;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.MeshOperations;

public class FurnitureBoxController : NetworkBehaviour, IUsePrimary, IUpdate, IScroll, IOnCtrl, IOnHeld, IOnDrop
{
    [SerializeField]
    private PlacableFurniture_Item furnitureItem;

    [SerializeField] private Material HologramMat;

    //Rotation & Snapping
    [Header("Rotation")]
    [SerializeField] float snappingRotationInterval = 22.5f;
    [SerializeField] float nonSnappintRotationInterval = 8f;

    private Mesh holoMesh;
    private RenderParams renderParams;

    private float rotation;
    private bool snappingEnabled;

    public const float PLACABLE_DISTANCE = 5;

    GridController grid;

    private void Awake() {
        renderParams = new RenderParams(HologramMat);

        if (furnitureItem != null)
            PopulateItem(furnitureItem);
        grid = GridController.Instance;

    }
    private void Start()

    {
        grid = GridController.Instance;
    }

    [Rpc(SendTo.Everyone)]
    public void SetItem_Rpc(string itemID, float rotation = 0)
    {
        furnitureItem = ItemDictionaryManager.RetrieveItem(itemID) is not PlacableFurniture_Item ? null : (PlacableFurniture_Item)ItemDictionaryManager.RetrieveItem(itemID);
        if (furnitureItem == null) return;

        this.rotation = rotation;
        PopulateItem(furnitureItem);
    }

    public void PopulateItem(PlacableFurniture_Item furnitureItem)
    {
        if (furnitureItem.FurniturePrefab.TryGetComponent(out MeshFilter meshFilter))
        {
            holoMesh = meshFilter.sharedMesh;
        }
    }

    public void OnHeld(PlayerHoldingManager holdingManager) {
        snappingEnabled = holdingManager.SnappingEnabled;
        grid.SetVisabiltay(snappingEnabled);
    }

    public void UsePrimary(PlayerHoldingManager holdingManager)
    {
        if (furnitureItem == null) return;

        Ray ray = new(holdingManager.CameraManager.CamTransform.position, holdingManager.CameraManager.CamTransform.forward);


        if (Physics.Raycast(ray, out RaycastHit hit, PLACABLE_DISTANCE, LayerMask.GetMask("Placeable")))
        {
            Vector3 position = snappingEnabled ? grid.HitToGrid(hit.point) : hit.point;
            if (Physics.OverlapBox(position + holoMesh.bounds.center + furnitureItem.FurniturePrefab.transform.position, holoMesh.bounds.size * 0.48f, Quaternion.Euler(0, rotation, 0)).Length == 0)
            {
                grid.SetVisabiltay(false);
                holdingManager.PlaceItem_Rpc(NetworkObject, furnitureItem.ItemID, position, Quaternion.Euler(0, rotation, 0));
            }
        }
    }

    public void OnScroll(PlayerHoldingManager manager, InputAction.CallbackContext context) 
    {
        if (furnitureItem == null) return;

        float dir = context.ReadValue<float>() > 0 ? 1 : -1;

        rotation += dir * (snappingEnabled ? snappingRotationInterval : nonSnappintRotationInterval);

    }

    public void OnCtrl(PlayerHoldingManager manager) {
        snappingEnabled = !snappingEnabled;
        manager.SnappingEnabled = snappingEnabled;
        grid.SetVisabiltay(snappingEnabled);
        if (snappingEnabled) rotation = Snapping.Snap(rotation, snappingRotationInterval); 
    }

    
    public void OnUpdate(PlayerHoldingManager holdingManager)
    {
        if (furnitureItem == null) return;
        if (!holdingManager.IsLocalPlayer) return;

        Ray ray = new(holdingManager.CameraManager.CamTransform.position, holdingManager.CameraManager.CamTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, PLACABLE_DISTANCE, LayerMask.GetMask("Placeable")))
        {
            Vector3 position = snappingEnabled ? grid.HitToGrid(hit.point) : hit.point;
            HologramMat.SetFloat("_OverlappingColliders", Physics.OverlapBox(position+ holoMesh.bounds.center + furnitureItem.FurniturePrefab.transform.position, holoMesh.bounds.size * 0.48f, Quaternion.Euler(0, rotation, 0)).Length);
            gizmoPos = position + holoMesh.bounds.center + furnitureItem.FurniturePrefab.transform.position;
            //Gizmos.draw
            foreach (MeshRenderer mesh in furnitureItem.FurniturePrefab.GetComponentsInChildren<MeshRenderer>())
            {
                if (!mesh.TryGetComponent<MeshFilter>(out MeshFilter filter)) continue;
                Graphics.RenderMesh(renderParams, filter.sharedMesh, 0, Matrix4x4.TRS(position+mesh.transform.position, mesh.transform.rotation * Quaternion.Euler(0, rotation, 0), Vector3.one));
            }

            //Graphics.RenderMesh(renderParams, holoMesh, 0, Matrix4x4.TRS(position, Quaternion.Euler(0, rotation, 0), Vector3.one));
        }
    }

    public void OnDrop(PlayerHoldingManager holdingManager)
    {
        grid.SetVisabiltay(false);
    }

    Vector3 gizmoPos = Vector3.zero;
    void OnDrawGizmosSelected() {
        if (holoMesh == null) return;

        // Draw a yellow cube at the transform position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(gizmoPos, holoMesh.bounds.size * 0.48f);
    }
}
