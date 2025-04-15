using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Rendering;

public class PlayerBuildingManager : NetworkBehaviour
{

    

    public enum mode
    {
        inactive, selectionMode, placementMode, destroyMode
    }

    private mode _Mode;

    [field: SerializeField]
    public mode Mode
    {
        get
        {
            return _Mode;
        }
        private set
        {
            if (value == _Mode) return;
            switch (_Mode)
            {
                case mode.placementMode:
                    grid.SetVisabiltay(false);
                    break;
            }
            switch (value)
            {
                case mode.placementMode:
                    grid.SetVisabiltay(true);
                    ui.GetComponent<CanvasGroup>().alpha = 0.7f;
                    break;
                case mode.destroyMode:
                    ui.GetComponent<CanvasGroup>().alpha = 0.4f;
                    break;
                default:
                    ui.GetComponent<CanvasGroup>().alpha = 1f;
                    break;
            }
            _Mode = value;
        }
    }




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

    private void Awake()
    {
        PlayerInputManager playerInputManager = GetComponent<PlayerInputManager>();

        playerInputManager.OnPerformPrimary += PerformPrimary;
        playerInputManager.OnPerformSecondary += PerformSecondary;
        playerInputManager.OnDismantle += PerformDismantle;

        playerInputManager.OnScroll += PerformScroll;

        playerInputManager.OnInteract += PerfromE;
        playerInputManager.OnQ += PerfromQ;

        playerInputManager.OnPerformCtrl += ToggleBuilding;


        CameraManager = GetComponentInChildren<PlayerCameraManager>();

        renderParams = new RenderParams(HologramMat);
        renderParams.matProps = new();

        if (furnitureItem != null)
            PopulateItem(furnitureItem);
        grid = GridController.Instance;
    }







    public void Update()
    {

        if (Mode == mode.placementMode)
            RenderPlacementHologram();
        else if (Mode == mode.destroyMode)
            RenderDestoryHoloGram();
    }



    [Rpc(SendTo.Everyone)]
    public void SetItem_Rpc(string itemID, float rotation = 0)
    {
        furnitureItem = ItemDictionaryManager.RetrieveItem(itemID) is not PlacableFurniture_Item ? null : (PlacableFurniture_Item)ItemDictionaryManager.RetrieveItem(itemID);
        if (furnitureItem == null) return;

        this.rotation = rotation;
        PopulateItem(furnitureItem);
    }

    [Rpc(SendTo.Server)]
    public void PlaceItem_Rpc(string itemId, Vector3 location, Quaternion rotation)
    {
        PlacableFurniture_Item placeableItem = ItemDictionaryManager.RetrieveItem(itemId) is not PlacableFurniture_Item ? null : (PlacableFurniture_Item)ItemDictionaryManager.RetrieveItem(itemId);
        Debug.Log($"Placing furntiure item {itemId}");

        if (placeableItem == null) return;

        NetworkObject instance = Instantiate(placeableItem.FurniturePrefab, location + placeableItem.FurniturePrefab.transform.position, rotation).GetComponent<NetworkObject>();
        instance.Spawn();

    }

    public void PopulateItem(PlacableFurniture_Item furnitureItem)
    {
        if (furnitureItem.FurniturePrefab.TryGetComponent(out PlaceableObject placeableMesh))
        {
            if (placeableMesh.BuildHologramMesh != null)
            {
                holoMesh = placeableMesh.BuildHologramMesh;
            }
            else if (furnitureItem.FurniturePrefab.TryGetComponent(out MeshFilter meshFilter))
            {
                holoMesh = meshFilter.sharedMesh;
            }
            else { holoMesh = furnitureItem.FurniturePrefab.GetComponentInChildren<MeshFilter>().sharedMesh; }
        }
    }


    public void BuildItem()
    {
        if (furnitureItem == null) return;

        Ray ray = new(CameraManager.CamTransform.position, CameraManager.CamTransform.forward);


        if (Physics.Raycast(ray, out RaycastHit hit, PLACABLE_DISTANCE, LayerMask.GetMask("Placeable")))
        {
            Vector3 position = grid.HitToGrid(hit.point);
            if (Physics.OverlapBox(position + holoMesh.bounds.center + furnitureItem.FurniturePrefab.transform.position, holoMesh.bounds.size * 0.48f, Quaternion.Euler(0, rotation, 0)).Length == 0)
            {
                PlaceItem_Rpc(furnitureItem.ItemID, position, Quaternion.Euler(0, rotation, 0));
            }
        }
    }

    private void RenderPlacementHologram()
    {
        if (furnitureItem == null) return;
        Ray ray = new(CameraManager.CamTransform.position, CameraManager.CamTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, PLACABLE_DISTANCE, LayerMask.GetMask("Placeable")))
        {
            Vector3 position = grid.HitToGrid(hit.point);
            renderParams.matProps.SetFloat("_OverlappingColliders", Physics.OverlapBox(position + holoMesh.bounds.center + furnitureItem.FurniturePrefab.transform.position, holoMesh.bounds.size * 0.48f, Quaternion.Euler(0, rotation, 0)).Length);
            gizmoPos = position + holoMesh.bounds.center + furnitureItem.FurniturePrefab.transform.position;
            //Gizmos.draw

            if (furnitureItem.FurniturePrefab.TryGetComponent(out PlaceableObject placeableMesh))
            {
                if (placeableMesh.InvertMaterial())
                {
                    renderParams.material.SetFloat("_Cull", (int)CullMode.Front);
                }
                else
                {
                    renderParams.material.SetFloat("_Cull", (int)CullMode.Back);

                }

                if (placeableMesh.BuildHologramMesh != null)
                {
                    Graphics.RenderMesh(renderParams, placeableMesh.BuildHologramMesh, 0, placeableMesh.GetMatrix(position, Quaternion.Euler(0, rotation, 0)));
                }
                else
                {
                    foreach (MeshRenderer mesh in furnitureItem.FurniturePrefab.GetComponentsInChildren<MeshRenderer>())
                    {
                        if (!mesh.TryGetComponent<MeshFilter>(out MeshFilter filter)) continue;
                        Graphics.RenderMesh(renderParams, filter.sharedMesh, 0, Matrix4x4.TRS(position + mesh.transform.position, mesh.transform.rotation * Quaternion.Euler(0, rotation, 0), mesh.transform.lossyScale));
                    }
                }
            }
        }
    }

    private void DestroyItem()
    {
        Ray ray = new(CameraManager.CamTransform.position, CameraManager.CamTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, PLACABLE_DISTANCE))
        {
            if (!hit.transform.root.TryGetComponent(out PlaceableObject placeable)) return;
            if (!placeable.CanRemove) return;

            NetworkObject obj = hit.transform.root.GetComponentInChildren<NetworkObject>();
            if (obj != null)
                Destroy_RPC(obj);
        }
    }
    [Rpc(SendTo.Server)]
    private void Destroy_RPC(NetworkObjectReference networkObject)
    {
        if (networkObject.TryGet(out NetworkObject obj))
        {
            if (obj.TryGetComponent(out Collider collider))
            {
                Collider[] colliders = Physics.OverlapSphere(obj.transform.position, collider.bounds.max.magnitude, LayerMask.GetMask("Default"));

                foreach (Collider overlappedCollider in colliders)
                {
                    if (overlappedCollider.TryGetComponent(out RigidbodyNetworkTransform rbNWT))
                    {
                        rbNWT.WakeUp();
                    }
                }
            }


            obj.Despawn();
        }
    }

    private void RenderDestoryHoloGram()
    {
        Ray ray = new(CameraManager.CamTransform.position, CameraManager.CamTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, PLACABLE_DISTANCE))
        {
            if (!hit.transform.root.TryGetComponent(out PlaceableObject placeable)) return;
            if (!placeable.CanRemove) return;

            Vector3 position = hit.point;
            renderParams.material.SetFloat("_Cull", (int)CullMode.Back);
            renderParams.matProps.SetFloat("_OverlappingColliders", 1);

            //Gizmos.draw
            foreach (MeshRenderer mesh in hit.transform.root.gameObject.GetComponentsInChildren<MeshRenderer>())
            {
                if (!mesh.TryGetComponent<MeshFilter>(out MeshFilter filter)) continue;
                Graphics.RenderMesh(renderParams, filter.sharedMesh, 0, Matrix4x4.TRS(mesh.transform.position, mesh.transform.rotation, mesh.transform.lossyScale));
            }

        }
    }

    #region input
    private void PerformPrimary(InputAction.CallbackContext context)
    {
        if (!(context.performed && context.interaction is PressInteraction)) return;
        switch (Mode)
        {
            case mode.selectionMode:
                SetItem_Rpc(ui.GetSelectedId());
                Mode = mode.placementMode;
                break;
            case mode.placementMode:
                BuildItem();
                break;
            case mode.destroyMode:
                DestroyItem();
                break;
        }
    }

    private void PerformSecondary(InputAction.CallbackContext context)
    {
        if (Mode == mode.placementMode || Mode == mode.destroyMode)
        {
            Mode = mode.selectionMode;
        }
    }

    private void PerformDismantle(InputAction.CallbackContext context)
    {
        if (Mode == mode.inactive) return;
        if (Mode != mode.destroyMode) Mode = mode.destroyMode;
        else if (Mode == mode.destroyMode) Mode = mode.selectionMode;
    }

    public void PerformScroll(InputAction.CallbackContext context)
    {
        if (Mode == mode.inactive) return;

        float dir = context.ReadValue<float>() > 0 ? -1 : 1;
        if (Mode == mode.selectionMode)
        {
            ui.ScrolPanel((int)dir);
        }
        else if (Mode == mode.placementMode)
        {
            if (furnitureItem == null) return;
            rotation += dir * 90;
        }
    }

    public void PerfromE(InputAction.CallbackContext context)
    {
        if (Mode == mode.selectionMode || Mode == mode.placementMode)
        {
            ui.MoveX(1);
            if (Mode == mode.placementMode) SetItem_Rpc(ui.GetSelectedId());
        }
    }

    public void PerfromQ(InputAction.CallbackContext context)
    {
        if (Mode == mode.selectionMode || Mode == mode.placementMode)
        {
            ui.MoveX(-1);
            if (Mode == mode.placementMode) SetItem_Rpc(ui.GetSelectedId());
        }
    }

    public void ToggleBuilding(InputAction.CallbackContext context)
    {

        //hide/show UI 
        //stop any proccesses if disabling


        if (Mode == mode.inactive) Mode = mode.selectionMode;
        else if (Mode != mode.inactive) Mode = mode.inactive;
        ui.SetVisabiltiy(Mode == mode.selectionMode);
    }

    #endregion

    Vector3 gizmoPos = Vector3.zero;
    void OnDrawGizmosSelected()
    {
        if (holoMesh == null) return;

        // Draw a yellow cube at the transform position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(gizmoPos, holoMesh.bounds.size * 0.48f);
    }

}
