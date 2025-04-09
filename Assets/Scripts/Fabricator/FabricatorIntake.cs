using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;

public class FabricatorIntake : NetworkBehaviour, IUseItem {

    Fabricator fabricator;

    private void Awake() {
        fabricator = transform.parent.GetComponent<Fabricator>();
    }

    private void OnTriggerEnter(Collider other) {
        if (!fabricator) return;
        if (!IsServer) return;

        TryProcessInput(other.gameObject);

        
    }

    public void OnItemUsed(PlayerHoldingManager holdingManager, Stock_Item shopProduct_Item) {
        NetworkObject nwObj = holdingManager.HeldObj;
        if (TryProcessInput(nwObj.gameObject)) {
            holdingManager.ClearHeldItem(holdingManager.transform.position, holdingManager.transform.eulerAngles);
        }
    }

    public bool CanUseItem(PlayerHoldingManager holdingManager, Stock_Item item)
    {
        return true;
    }

    public bool TryProcessInput(GameObject other) {
        if (other.TryGetComponent(out StockItem stockItem)) {
            if (stockItem.Item == null) return false;
            if (!stockItem.TryGetComponent(out NetworkObject nwObj)) return false;
            if (!nwObj.IsSpawned) return false;
            if (!fabricator.MakeItem(stockItem.Item)) return false;

            /*ClipData clipData = eatingClip.GetClip(1, true);

            audioSource.pitch = clipData.Pitch;
            audioSource.PlayOneShot(clipData.Clip);*/
            other.gameObject.GetComponent<NetworkObject>().Despawn();
            return true;
        } else return false;

    }

    public InteractionContext OnViewWithItem(PlayerHoldingManager holdingManager, Stock_Item item)
    {
        if (CanUseItem(holdingManager, item))
        {
            return new(true);
        }

        return new(false);
    }

    public void OnUnview()
    {

    }
}
