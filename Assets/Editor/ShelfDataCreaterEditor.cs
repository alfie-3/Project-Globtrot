using Codice.CM.Common.Merge;
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShelfDataCreater))]
class ShelfDataCreaterEditor: Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ShelfDataCreater shelfData = (ShelfDataCreater)target;

        if (GUILayout.Button("Calculate Values"))
        {
            Debug.Log("Calculating: " + target.name);
            shelfData.objectBounds = shelfData.GetComponent<MeshFilter>().sharedMesh.bounds.size;
            shelfData.offset = (shelfData.GetComponent<MeshFilter>().sharedMesh.bounds.size * 0.5f) - shelfData.GetComponent<MeshFilter>().sharedMesh.bounds.center;
            shelfData.gizmoOffset = shelfData.GetComponent<MeshFilter>().sharedMesh.bounds.center;
            //shelfData.HapyBounds = new(0, 7, 2);
            shelfData.transform.localPosition = shelfData.offset;

            shelfData.stackBounds.x = (float)Math.Floor((double)(shelfData.transform.parent.GetComponent<MeshFilter>().sharedMesh.bounds.size.x / shelfData.objectBounds.x));
            shelfData.stackBounds.y = (float)Math.Floor((double)(shelfData.transform.parent.GetComponent<MeshFilter>().sharedMesh.bounds.size.y / shelfData.objectBounds.y));
            shelfData.stackBounds.z = (float)Math.Floor((double)(shelfData.transform.parent.GetComponent<MeshFilter>().sharedMesh.bounds.center.z / shelfData.objectBounds.z));
            //Debug.Log(shelfData.transform.parent.GetComponent<MeshFilter>().sharedMesh.bounds.size);
            //Debug.Log(shelfData.transform.parent.GetComponent<MeshFilter>().sharedMesh.bounds.center);
            //Debug.Log((shelfData.transform.parent.GetComponent<MeshFilter>().sharedMesh.bounds.size.y / shelfData.objectBounds.y));
        }
        if (GUILayout.Button("Even Out Placement Values"))
        {
            shelfData.EvenOutOffset.x = (shelfData.transform.parent.GetComponent<MeshFilter>().sharedMesh.bounds.size.x % shelfData.objectBounds.x) * 0.5f;
            shelfData.EvenOutOffset.y = (shelfData.transform.parent.GetComponent<MeshFilter>().sharedMesh.bounds.size.y % shelfData.objectBounds.y) * 0.5f;
            shelfData.EvenOutOffset.z = (shelfData.transform.parent.GetComponent<MeshFilter>().sharedMesh.bounds.center.z % shelfData.objectBounds.z)*0.5f;
        }
        if (GUILayout.Button("Save Scriptable Object values"))
        {
            ShopProduct_Item item = (ShopProduct_Item)Resources.Load($"Items/{shelfData.ItemScriptableObjectName}", typeof(ShopProduct_Item));
            if (item != null)
            {
                Debug.LogError($"Unable to locate {shelfData.ItemScriptableObjectName}, make sure there is a scritable object in the Resources/Items folder with that name");
                return;
            }
            item.ObjectBounds = shelfData.objectBounds;
            if (shelfData.transform.parent.GetComponent<StockShelfController>().normalShelfSize)
                item.StackBounds = shelfData.stackBounds;
            else
                item.BigStackBounds = shelfData.stackBounds;
        }
    }
}