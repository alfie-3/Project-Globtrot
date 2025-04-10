using Assets.Scripts.Interfaces;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerHoldingManager))]
public class PlayerInteractionManager : NetworkBehaviour
{
    [SerializeField] float InteractionDistance = 5f;
    [SerializeField] PlayerCameraManager cameraManager;
    [Space]
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask holdingInteractableLayer;

    PlayerHoldingManager holdingManager;
    PlayerBuildingManager buildingManager;

    GameObject interactableGO;

    public Action<bool, InteractionContext> OnSetObjectViewed;

    private void Awake()
    {
        if (TryGetComponent(out PlayerInputManager inputManager))
        {
            inputManager.OnInteract += Interact;
            inputManager.OnDismantle += Dismantle;
        }

        holdingManager = GetComponent<PlayerHoldingManager>();
        buildingManager = GetComponent<PlayerBuildingManager>();
        cameraManager = GetComponentInChildren<PlayerCameraManager>();
    }

    private void Update()
    {
        if (cameraManager == null) return;
        if (!IsLocalPlayer) return;
        if (buildingManager.Mode != PlayerBuildingManager.mode.inactive) return;

        CheckInteractables();
    }

    public LayerMask CurrentLayerMask()
    {
        return holdingManager.HeldObj == null ? interactableLayer : holdingInteractableLayer;
    }

    public void CheckInteractables()
    {
        Ray ray = new(cameraManager.CamTransform.position, cameraManager.CamTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, InteractionDistance, CurrentLayerMask(), QueryTriggerInteraction.Collide))
        {
            if (interactableGO == hit.transform.gameObject) return;

            if (hit.collider.gameObject.TryGetComponent(out IInteractable _))
            {
                View(hit.collider.gameObject);
            }
            else if (hit.collider.gameObject.TryGetComponent(out IUseItem _))
            {
                View(hit.collider.gameObject);
            }
            else
            {
                Unview();
            }
        }
        else
        {
            Unview();
        }
    }

    public void View(GameObject go)
    {
        if (holdingManager.HeldObj == null)
        {
            if (go.TryGetComponent(out IViewable view))
            {
                InteractionContext context = view.OnView();
                OnSetObjectViewed.Invoke(true, context);
            }
            else
                OnSetObjectViewed.Invoke(true, InteractionContext.DefaultContext);

        }
        else
        {
            if (go.TryGetComponent(out IUseItem useItem))
            {
                if (holdingManager.HeldObj.TryGetComponent(out StockItem item))
                {
                    InteractionContext context = useItem.OnViewWithItem(holdingManager, item.Item);

                    if (context.InteractionAvailable)
                        OnSetObjectViewed.Invoke(true, context);
                }
            }
        }


        interactableGO = go;
    }


    public void Unview()
    {
        if (interactableGO != null)
        {
            if (interactableGO.TryGetComponent(out IViewable view))
            {
                view.OnUnview();
            }
            if (interactableGO.TryGetComponent(out IUseItem itemUsage))
            {
                itemUsage.OnUnview();
            }

            OnSetObjectViewed.Invoke(false, default);

            interactableGO = null;
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (buildingManager.Mode != PlayerBuildingManager.mode.inactive) return;

        Ray ray = new(cameraManager.CamTransform.position, cameraManager.CamTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, InteractionDistance, CurrentLayerMask(), QueryTriggerInteraction.Collide))
        {
            if (holdingManager.HeldObj == null)
            {
                HandleInteract(hit);
            }
            else
            {
                HandleInteractWithItem(hit);
            }
        }
    }

    public void HandleInteract(RaycastHit hit)
    {
        if (hit.collider.gameObject.TryGetComponent(out IInteractable interactable))
        {
            interactable.OnInteract(this);
        }
    }

    public void HandleInteractWithItem(RaycastHit hit)
    {
        if (hit.collider.gameObject.TryGetComponent(out IUseItem interactable))
        {
            if (holdingManager.HeldObj.TryGetComponent(out StockItem item))
            {
                interactable.OnItemUsed(holdingManager, item.Item);
            }
        }
    }

    public void Dismantle(InputAction.CallbackContext context)
    {
        Ray ray = new(cameraManager.CamTransform.position, cameraManager.CamTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, InteractionDistance, interactableLayer, QueryTriggerInteraction.Collide))
        {
            if (hit.collider.gameObject.TryGetComponent(out IDismantleable dismantleable))
            {
                dismantleable.OnDismantle(this);
            }
        }
    }
}
