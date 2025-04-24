using Unity.Netcode;
using UnityEngine;

public class ContainerContents : NetworkBehaviour
{
    public Contents Contents {  get; private set; } = new Contents();

    public bool TryAddItem(Stock_Item item, int quantity = 1)
    {
        if (Contents.TryAddItem(item, quantity))
        {
            ReplicateContentsItem_Rpc(item.ItemID, Contents.ContentsDictionary[item]);
            return true;
        }

        return false;
    }

    public bool TryRemoveItem(Stock_Item item, int quantity = 1)
    {
        if (Contents.TryRemoveItem(item.ItemID, quantity))
        {
            if (!Contents.ContentsDictionary.ContainsKey(item))
            {
                ReplicateContentsItemRemove_Rpc(item.ItemID);
            }
            else
            {
                ReplicateContentsItem_Rpc(item.ItemID, Contents.ContentsDictionary[item]);
            }

            return true;
        }

        return false;
    }

    [Rpc(SendTo.NotServer)]
    public void ReplicateContentsItem_Rpc(string id, int quanitity)
    {
        if (IsServer) return;

        Stock_Item item = (Stock_Item)ItemDictionaryManager.RetrieveItem(id);
        if (item == null) return;

        if (Contents.ContentsDictionary.TryGetValue(item, out int value))
        {
            Contents.ContentsDictionary[item] = quanitity;
        }
        else
        {
            Contents.ContentsDictionary.Add(item, quanitity);
        }
    }

    [Rpc(SendTo.NotServer)]
    public void ReplicateContentsItemRemove_Rpc(string id)
    {
        if (IsServer) return;

        Stock_Item item = (Stock_Item)ItemDictionaryManager.RetrieveItem(id);
        if (item == null) return;

        Contents.ContentsDictionary.Remove(item);
    }
}
