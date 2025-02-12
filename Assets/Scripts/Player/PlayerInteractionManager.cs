using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionManager : NetworkBehaviour
{
    [SerializeField] float InteractionDistance = 5f;
    [SerializeField] PlayerCameraManager cameraManager;
    [Space]
    [SerializeField] LayerMask interactableLayer;

    IInteractable currentInteractable;

    private void Awake()
    {
        if (TryGetComponent(out PlayerInputManager inputManager))
        {
            inputManager.OnInteract += Interact;
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

        if (Physics.Raycast(ray, out RaycastHit hit, InteractionDistance, interactableLayer, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.gameObject.TryGetComponent(out IInteractable interactable))
            {
                if (interactable != currentInteractable)
                    interactable.OnView();

                currentInteractable = interactable;
            }
        }
        else
        {
            if (currentInteractable != null)
            {
                currentInteractable.OnUnview();
                currentInteractable = null;
            }
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        currentInteractable?.OnInteract(this);
    }
}
