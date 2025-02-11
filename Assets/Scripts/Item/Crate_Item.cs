using Unity.AppUI.UI;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "New Placable Item", menuName = "Items/Crate")]
public class Crate_Item : Placable_Item {
    public string ItemInsideId;
    public override void OnSecondary(PlayerHoldingManager holdingManager) {
        //base.OnSecondary(holdingManager);
        Debug.Log("Throw1");
        holdingManager.HeldObj.OnDrop(holdingManager);
        holdingManager.HeldObj.GetComponent<Rigidbody>().AddForce(holdingManager.HeldObj.transform.position - holdingManager.transform.position * 4);
    }
    public override void OnPrimary(PlayerHoldingManager holdingManager) {
        base.OnPrimary(holdingManager);
        PlaceablePrefab = (ItemDictionaryManager.RetrieveItem(ItemInsideId) is not Placable_Item ? null : (Placable_Item)ItemDictionaryManager.RetrieveItem(ItemInsideId)).PlaceablePrefab;
        if (PlaceablePrefab != null)
            PlaceItem_Rpc(holdingManager);
        holdingManager.DestoryHeldObject();
    }
    public void Set 
}
