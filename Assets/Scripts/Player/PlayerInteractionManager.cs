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

    PlayerHoldingManager holdingManager;

    GameObject interactableGO;

    public Action<bool> OnSetObjectViewed;

    private void Awake()
    {
        if (TryGetComponent(out PlayerInputManager inputManager))
        {
            inputManager.OnInteract += Interact;
            inputManager.OnDismantle += Dismantle;
        }

        holdingManager = GetComponent<PlayerHoldingManager>();
        cameraManager = GetComponentInChildren<PlayerCameraManager>();
    }

    private void Update()
    {
        if (cameraManager == null) return;
        if (!IsLocalPlayer) return;

        CheckInteractables();
    }

    public void CheckInteractables()
    {
        Ray ray = new(cameraManager.CamTransform.position, cameraManager.CamTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, InteractionDistance, interactableLayer, QueryTriggerInteraction.Collide))
        {
            if (hit.collider.gameObject.TryGetComponent(out IInteractable interacrable))
            {
                View(hit.collider.gameObject, interacrable);
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

    public void View(GameObject go, IInteractable interactable)
    {
        if (interactableGO == go) return;

        if (go.TryGetComponent(out IViewable view))
        {
            view.OnView();
        }

        OnSetObjectViewed.Invoke(true);

        interactableGO = go;
    }

    public void Unview()
    {
        if (interactableGO != null)
        {
            if (interactableGO.TryGetComponent(out IViewable view))
            {
                view.OnView();
            }

            OnSetObjectViewed.Invoke(false);

            interactableGO = null;
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (GetComponent<PlayerBuildingManager>().buildingManagerActive) return;

        Ray ray = new(cameraManager.CamTransform.position, cameraManager.CamTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, InteractionDistance, interactableLayer, QueryTriggerInteraction.Collide))
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
