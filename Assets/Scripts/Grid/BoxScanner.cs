using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class BoxScanner : MonoBehaviour
{

    [SerializeField] UI_OrderScreen[] screens = new UI_OrderScreen[2];

    private void OnTriggerEnter(Collider other)
    {
        IContents boxContents = null;
        if (other.TryGetComponent(out OrderContainerBox orderContainerbox)) { boxContents = orderContainerbox; }
        if (boxContents == null) return;

        CancelInvoke();
        ham(boxContents.Contents);
    }

    void ham(Contents contents)
    {
        List<OrderItem> orfers = new();
        contents.ContentsDictionary.ToList().ForEach(pair => { orfers.Add(new(pair.Key, pair.Value)); });
        Order order = new(3, orfers, 42);
        screens[0].AddOrder(order); screens[1].AddOrder(order);
    }

    private void OnTriggerExit(Collider other)
    {
        IContents boxContents = null;
        if (other.TryGetComponent(out OrderContainerBox orderContainerbox)) { boxContents = orderContainerbox; }
        if (boxContents == null) return;
        
        Invoke("ClearScreens",2);
    }

    void ClearScreens() {screens[0].ClearList(); screens[1].ClearList();}
}
