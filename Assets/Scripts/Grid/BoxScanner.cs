using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class BoxScanner : MonoBehaviour
{

    [SerializeField] UI_DisplayContents[] screens = new UI_DisplayContents[2];
    [SerializeField] float scannerResetTimer = 5f;

    private void OnTriggerEnter(Collider other)
    {
        IContents boxContents = null;
        if (other.TryGetComponent(out OrderContainerBox orderContainerbox)) { boxContents = orderContainerbox; }
        if (boxContents == null) return;

        CancelInvoke();
        UpdateScreens(boxContents.Contents);
    }

    void UpdateScreens(Contents contents)
    {
        screens.ToList().ForEach(s => {s.AddContents(contents);});
    }

    private void OnTriggerExit(Collider other)
    {
        IContents boxContents = null;
        if (other.TryGetComponent(out OrderContainerBox orderContainerbox)) { boxContents = orderContainerbox; }
        if (boxContents == null) return;
        
        Invoke("ClearScreens",scannerResetTimer);
    }

    void ClearScreens() { screens.ToList().ForEach(s => { s.ClearList(); }); }
}
