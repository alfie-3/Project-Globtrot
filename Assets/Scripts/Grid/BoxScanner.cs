using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class BoxScanner : MonoBehaviour
{

    [SerializeField] UI_DisplayContents[] screens = new UI_DisplayContents[2];

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
        screens[0].AddContents(contents); screens[1].AddContents(contents);
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
