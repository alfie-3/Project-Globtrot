using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerBuildingManager : NetworkBehaviour {

    public bool buildingManagerActive;
    public bool selectionMode;

    public UI_BuildingSelection ui;


    [SerializeField]
    private PlacableFurniture_Item furnitureItem;

    [SerializeField] private Material HologramMat;

    private Mesh holoMesh;
    private RenderParams renderParams;

    private float rotation;

    public const float PLACABLE_DISTANCE = 5;

    GridController grid;

    
    public PlayerCameraManager CameraManager { get; private set; }

    private void Awake() {
        PlayerInputManager playerInputManager = GetComponent<PlayerInputManager>();

        playerInputManager.OnPerformPrimary += PerformPrimary;
        playerInputManager.OnPerformSecondary += PerformSecondary;
        playerInputManager.OnScroll += PerformScroll;

        playerInputManager.OnInteract += PerfromE;
        playerInputManager.OnQ += PerfromQ;

        playerInputManager.OnPerformCtrl += ToggleBuilding;

        CameraManager = GetComponentInChildren<PlayerCameraManager>();


        renderParams = new RenderParams(HologramMat);

        if (furnitureItem != null)
            PopulateItem(furnitureItem);
        grid = GridController.Instance;
    }

    [Rpc(SendTo.Everyone)]
    public void SetItem_Rpc(string itemID, float rotation = 0) {
        furnitureItem = ItemDictionaryManager.RetrieveItem(itemID) is not PlacableFurniture_Item ? null : (PlacableFurniture_Item)ItemDictionaryManager.RetrieveItem(itemID);
        if (furnitureItem == null) return;

        this.rotation = rotation;
        PopulateItem(furnitureItem);
    }

    [Rpc(SendTo.Server)]
    public void PlaceItem_Rpc(string itemId, Vector3 location, Quaternion rotation) {
        PlacableFurniture_Item placeableItem = ItemDictionaryManager.RetrieveItem(itemId) is not PlacableFurniture_Item ? null : (PlacableFurniture_Item)ItemDictionaryManager.RetrieveItem(itemId);
        Debug.Log($"Placing furntiure item {itemId}");

        if (placeableItem == null) return;

        NetworkObject instance = Instantiate(placeableItem.FurniturePrefab, location + placeableItem.FurniturePrefab.transform.position, rotation).GetComponent<NetworkObject>();
        instance.Spawn();

    }
    public void PopulateItem(PlacableFurniture_Item furnitureItem) {
        if (furnitureItem.FurniturePrefab.TryGetComponent(out MeshFilter meshFilter)) {
            holoMesh = meshFilter.sharedMesh;
        }
    }


    public void BuildItem() {
        if (furnitureItem == null) return;

        Ray ray = new(CameraManager.CamTransform.position,  CameraManager.CamTransform.forward);


        if (Physics.Raycast(ray, out RaycastHit hit, PLACABLE_DISTANCE, LayerMask.GetMask("Placeable"))) {
            Vector3 position = grid.HitToGrid(hit.point);
            if (Physics.OverlapBox(position + holoMesh.bounds.center + furnitureItem.FurniturePrefab.transform.position, holoMesh.bounds.size * 0.48f, Quaternion.Euler(0, rotation, 0)).Length == 0) {
                PlaceItem_Rpc(furnitureItem.ItemID, position, Quaternion.Euler(0, rotation, 0));
            }
        }
    }

    private void PerformPrimary(InputAction.CallbackContext context)
    {
        if (!(context.performed && context.interaction is PressInteraction)) return;
        if (!buildingManagerActive) return;
        if (!selectionMode) { BuildItem(); return;}
        SetItem_Rpc(ui.GetSelectedId());
        selectionMode = false;
        grid.SetVisabiltay(!selectionMode);
    }

    private void PerformSecondary(InputAction.CallbackContext context)
    {
        if (!buildingManagerActive) return;

        selectionMode = true;
        grid.SetVisabiltay(!selectionMode);
    }

    public void PerformScroll(InputAction.CallbackContext context) {
        if (!buildingManagerActive) return;

        float dir = context.ReadValue<float>() > 0 ? -1 : 1;
        if (selectionMode) {
            ui.ScrolPanel((int)dir);
        } else {
            if (furnitureItem == null) return;
            rotation += dir * 90;
        }
    }

    public void PerfromE(InputAction.CallbackContext context)
    {
        if (!selectionMode) return;
        ui.MoveX(1);
    }

    public void PerfromQ(InputAction.CallbackContext context)
    {
        if (!selectionMode) return;
        ui.MoveX(-1);
    }

    public void ToggleBuilding(InputAction.CallbackContext context) 
    {
        buildingManagerActive = !buildingManagerActive;
        selectionMode = buildingManagerActive;
        ui.SetVisabiltiy(buildingManagerActive);
        grid.SetVisabiltay(false);
        //hide/show UI 
        //stop any proccesses if disabling
    }
    public void OnCtrl() {



        /*snappingEnabled = !snappingEnabled;
        manager.SnappingEnabled = snappingEnabled;
        grid.SetVisabiltay(snappingEnabled);
        if (snappingEnabled) rotation = Snapping.Snap(rotation, snappingRotationInterval);*/
    }

    public void Update() {
        if (!buildingManagerActive) return;
        if(selectionMode) return;
        if (furnitureItem == null) return;

        Ray ray = new(CameraManager.CamTransform.position, CameraManager.CamTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, PLACABLE_DISTANCE, LayerMask.GetMask("Placeable"))) {
            Vector3 position = grid.HitToGrid(hit.point);
            HologramMat.SetFloat("_OverlappingColliders", Physics.OverlapBox(position + holoMesh.bounds.center + furnitureItem.FurniturePrefab.transform.position, holoMesh.bounds.size * 0.48f, Quaternion.Euler(0, rotation, 0)).Length);
            gizmoPos = position + holoMesh.bounds.center + furnitureItem.FurniturePrefab.transform.position;
            //Gizmos.draw
            foreach (MeshRenderer mesh in furnitureItem.FurniturePrefab.GetComponentsInChildren<MeshRenderer>()) {
                if (!mesh.TryGetComponent<MeshFilter>(out MeshFilter filter)) continue;
                Graphics.RenderMesh(renderParams, filter.sharedMesh, 0, Matrix4x4.TRS(position + mesh.transform.position, mesh.transform.rotation * Quaternion.Euler(0, rotation, 0), Vector3.one));
            }

            //Graphics.RenderMesh(renderParams, holoMesh, 0, Matrix4x4.TRS(position, Quaternion.Euler(0, rotation, 0), Vector3.one));
        }
    }

    Vector3 gizmoPos = Vector3.zero;
    void OnDrawGizmosSelected() {
        if (holoMesh == null) return;

        // Draw a yellow cube at the transform position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(gizmoPos, holoMesh.bounds.size * 0.48f);
    }
}
