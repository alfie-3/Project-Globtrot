using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

[RequireComponent(typeof(ItemHolder))]
public class StockBoxController : NetworkBehaviour, IUsePrimary, IUseSecondary
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] ItemHolder holder;


    public void Awake()
    {
        //ItemQuantity.OnValueChanged += (() => text.text = $"{ItemId.Value.ToString()} \n {ItemQuantity.Value.ToString()}");
        
        holder.ItemQuantity.OnValueChanged += (previousValue, newValue) => text.text = $"{holder.ItemId.Value}\n{newValue}";

    }
    void UpdateText(int oldI, int newI)
    {
        text.text = $"{holder.ItemId.Value}\n{newI}";
    }

    public void UsePrimary(PlayerHoldingManager holdingManager) {
        Debug.Log("stockboxPri");
        //Debug.Log(ItemId.Value.ToString());
        if (holder.IsEmpty) return;



        Ray ray = new(holdingManager.CameraManager.CamTransform.position, holdingManager.CameraManager.CamTransform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, 5);
        if(Physics.Raycast(ray, out RaycastHit hit,5,LayerMask.GetMask("ItemShelf"))) 
        {
            if (hit.collider.TryGetComponent<ItemHolder>(out ItemHolder shelfItemHolder)) {
                ItemHolder.TransferItems(holder, shelfItemHolder);
            }
        }
    }

    public void UseSecondary(PlayerHoldingManager holdingManager)
    {
        Debug.Log("stockboxPri");
        //Debug.Log(ItemId.Value.ToString());
        //if (!holder.IsEmpty) return;


        Ray ray = new(holdingManager.CameraManager.CamTransform.position, holdingManager.CameraManager.CamTransform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, 5);
        if (Physics.Raycast(ray, out RaycastHit hit, 5, LayerMask.GetMask("ItemShelf")))
        {
            if (hit.collider.TryGetComponent<ItemHolder>(out ItemHolder shelfItemHolder))
            {
                ItemHolder.TransferItems(shelfItemHolder, holder);
            }
        }
    }
}
