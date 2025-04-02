using Assets.Scripts.Interfaces;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionManager : NetworkBehaviour
{
    [SerializeField] float InteractionDistance = 5f;
    [SerializeField] PlayerCameraManager cameraManager;
    [Space]
    [SerializeField] LayerMask interactableLayer;

    IViewable currentViewable;

    private void Awake()
    {
        if (TryGetComponent(out PlayerInputManager inputManager))
        {
            inputManager.OnInteract += Interact;
            inputManager.OnDismantle += Dismantle;
        }

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
            if (hit.collider.gameObject.TryGetComponent(out IViewable viewable))
            {
                if (currentViewable != viewable)
                    viewable.OnView();

                currentViewable = viewable;
            }
        }
        else
        {
            if (currentViewable != null)
            {
                currentViewable.OnUnview();
                currentViewable = null;
            }
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (GetComponent <PlayerBuildingManager>().buildingManagerActive) return;

        Ray ray = new(cameraManager.CamTransform.position, cameraManager.CamTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, InteractionDistance, interactableLayer, QueryTriggerInteraction.Collide))
        {
            if (hit.collider.gameObject.TryGetComponent(out IInteractable interactable))
            {
                interactable.OnInteract(this);
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
