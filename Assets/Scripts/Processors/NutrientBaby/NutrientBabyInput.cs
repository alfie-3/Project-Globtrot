using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;

public class NutrientBabyInput : NetworkBehaviour, IUseItem 
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClipRandomData eatingClip;

    NutrientBaby nutrientBaby;
    private void Awake()
    {
        nutrientBaby = transform.parent.GetComponent<NutrientBaby>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!nutrientBaby) return;
        if (!IsServer) return;

        TryProcessInput(other.gameObject);
    }

    public bool CanUseItem(PlayerHoldingManager holdingManager, Stock_Item item)
    {
        return nutrientBaby.CheckStock(item);
    }

    public void OnItemUsed(PlayerHoldingManager holdingManager, Stock_Item shopProduct_Item)
    {
        NetworkObject nwObj = holdingManager.HeldObj;
        if (TryProcessInput(nwObj.gameObject))
        {
            holdingManager.ClearHeldItem(transform.position, transform.rotation.eulerAngles);
        }
    }

    public bool TryProcessInput(GameObject other)
    {
        if (other.TryGetComponent(out StockItem stockItem))
        {
            if (stockItem.Item == null) return false;
            if (!stockItem.TryGetComponent(out NetworkObject nwObj)) return false;
            if (!nwObj.IsSpawned) return false;
            if (!nutrientBaby.TryFeed(nwObj, stockItem.Item)) return false;

            ClipData clipData = eatingClip.GetClip(1, true);

            audioSource.pitch = clipData.Pitch;
            audioSource.PlayOneShot(clipData.Clip);

            return true;
        }
        else return false;

    }

    public InteractionContext OnViewWithItem(PlayerHoldingManager holdingManager, Stock_Item item)
    {
        if (CanUseItem(holdingManager, item))
        {
            return new(true, "Feed");
        }

        return new(false);
    }

    public void OnUnview()
    {
        
    }
}
