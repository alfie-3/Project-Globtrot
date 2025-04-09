using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OrderContainerBox : NetworkBehaviour, IContents, IOnHeld, IUseItem
{
    [SerializeField] Contents boxContents;
    public Contents Contents => boxContents;
    [Space]
    [SerializeField] Animator boxAnimator;
    bool isOpen = false;

    private RigidbodyNetworkTransform rb;
    private Pickup_Interactable pickupInteractable;

    private void Awake()
    {
        rb = GetComponent<RigidbodyNetworkTransform>();
        pickupInteractable = GetComponent<Pickup_Interactable>();
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
}

[System.Serializable]
public class Contents
{
    [SerializeField] int maxContentsAmount = 4;
    bool useLimit = true;
    public bool AllowItems = true;

    public Dictionary<Stock_Item, int> ContentsDictionary { get; private set; } = new Dictionary<Stock_Item, int>();

    public int Count => ContentsDictionary.Count;

    public Contents()
    {
        ContentsDictionary = new Dictionary<Stock_Item, int>();
    }

    public Contents(int maxContentAmount)
    {
        ContentsDictionary = new Dictionary<Stock_Item, int>();

        if (maxContentAmount <= 0)
        {
            useLimit = false;
        }
        else
        {
            this.maxContentsAmount = maxContentAmount;
        }
    }

    public bool TryAddItem(Stock_Item item, int quantity = 1)
    {
        if (!AllowItems) return false;

        if (ContentsDictionary.Count > maxContentsAmount && useLimit) return false;

        if (ContentsDictionary.TryAdd(item, quantity)) return true;

        ContentsDictionary[item] += quantity;
        return true;
    }

    public bool TryRemoveItem(string id, int quantity = 1)
    {
        if (ContentsDictionary.Count == 0) return false;

        Stock_Item item = (Stock_Item)ItemDictionaryManager.RetrieveItem(id);
        if (item == null) return false;

        if (ContentsDictionary.TryGetValue(item, out int value))
        {
            value -= quantity;

            if (value <= 0)
                ContentsDictionary.Remove(item);

            return true;
        }

        return false;
    }
}

public interface IContents
{
    public Contents Contents { get; }
}