using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OrderContainerBox : NetworkBehaviour, IContents, IOnHeld
{
    [SerializeField] Contents boxContents;
    public Contents Contents => boxContents;

    [SerializeField] Animator boxAnimator;

    bool isOpen = false;

    private Rigidbody rigidbody;
    private Pickup_Interactable pickupInteractable;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
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
        if (rigidbody.isKinematic) return;

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
}

[System.Serializable]
public class Contents
{
    [SerializeField] int maxContentsAmount = 4;
    bool useLimit = true;
    public bool AllowItems = true;

    public Dictionary<string, int> ContentsDictionary { get; private set; } = new Dictionary<string, int>();

    public int Count => ContentsDictionary.Count;

    public Contents()
    {
        ContentsDictionary = new Dictionary<string, int>();
    }

    public Contents(int maxContentAmount)
    {
        ContentsDictionary = new Dictionary<string, int>();

        if (maxContentAmount <= 0)
        {
            useLimit = false;
        }
        else
        {
            this.maxContentsAmount = maxContentAmount;
        }
    }

    public bool TryAddItem(string id, int quantity = 1)
    {
        if (!AllowItems) return false;

        if (ContentsDictionary.Count > maxContentsAmount && useLimit) return false;

        if (ContentsDictionary.TryAdd(id, quantity)) return true;

        ContentsDictionary[id] += quantity;
        return true;
    }

    public bool TryRemoveItem(string name, int quantity = 1)
    {
        if (ContentsDictionary.Count == 0) return false;

        if (ContentsDictionary.TryGetValue(name, out int value))
        {
            value -= quantity;

            if (value <= 0)
                ContentsDictionary.Remove(name);

            return true;
        }

        return false;
    }
}

public interface IContents
{
    public Contents Contents { get; }
}