using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(ContainerContents))]
public class OrderContainerBox : NetworkBehaviour, IContents, IOnHeld, IUseItem
{
    [SerializeField] ContainerContents boxContents;
    public Contents Contents => boxContents.Contents;
    [Space]
    [SerializeField] ParticleSystem inputParticle;
    [SerializeField] Animator boxAnimator;
    bool isOpen = false;

    private RigidbodyNetworkTransform rb;
    private Pickup_Interactable pickupInteractable;

    private void Awake()
    {
        rb = GetComponent<RigidbodyNetworkTransform>();
        pickupInteractable = GetComponent<Pickup_Interactable>();

        boxContents.OnItemAdded += PlayAddAnimation;
    }

    public void SetOpen(bool open)
    {
        if (open)
        {
            if (isOpen == true) return;
            Contents.AllowItems = true;
            boxAnimator.Play("Open");
        }
        else
        {
            if (isOpen == false) return;
            Contents.AllowItems = false;
            boxAnimator.Play("Close");
        }

        isOpen = open;
    }

    private void FixedUpdate()
    {
        CheckForOpen();
    }

    public void CheckForOpen()
    {
        if (rb.IsSleeping) return;

        if (Vector3.Dot(transform.up, Vector3.up) < 0.5 || pickupInteractable.PickedUp.Value == true)
        {
            SetOpen(false);
            return;
        }

        var mask = ~0;
        Ray ray = new(transform.position, -transform.up);
        if (Physics.Raycast(ray, out RaycastHit hit, 1, mask, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.TryGetComponent(out WorkTable workTable))
            {
                SetOpen(true);
            }
            else
            {
                SetOpen(false);
            }
        }
    }

    public void OnHeld(PlayerHoldingManager manager)
    {
        SetOpen(false);
    }

    public void OnItemUsed(PlayerHoldingManager holdingManager, Stock_Item shopProduct_Item)
    {
        Bounds heldObjBounds = holdingManager.HeldObj.GetComponent<MeshRenderer>().bounds;

        Vector3 dropPoint = transform.position;
        dropPoint.y += (heldObjBounds.extents.y) + 0.5f;

        holdingManager.ClearHeldItem(dropPoint, holdingManager.HeldObj.transform.rotation.eulerAngles);
    }

    public bool CanUseItem(PlayerHoldingManager holdingManager, Stock_Item item)
    {
        return true;
    }

    public InteractionContext OnViewWithItem(PlayerHoldingManager holdingManager, Stock_Item item)
    {
        return new(true, "Pack");
    }

    public void OnUnview()
    {
        
    }

    public void PlayAddAnimation()
    {
        inputParticle.Play();
        boxAnimator.SetTrigger("Expand");
    }
}