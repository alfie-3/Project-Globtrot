using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShelfDataCreater))]
class ShelfDataCreaterEditor: Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ShelfDataCreater shelfData = (ShelfDataCreater)target;

        if (GUILayout.Button("Test"))
        {
            Debug.Log("It's alive: " + target.name);
            shelfData.objectBounds = shelfData.GetComponent<MeshFilter>().sharedMesh.bounds.size;
            shelfData.offset = (shelfData.GetComponent<MeshFilter>().sharedMesh.bounds.size * 0.5f) - shelfData.GetComponent<MeshFilter>().sharedMesh.bounds.center;
            shelfData.gizmoOffset = shelfData.GetComponent<MeshFilter>().sharedMesh.bounds.center;
            //shelfData.HapyBounds = new(0, 7, 2);
            shelfData.transform.localPosition = Vector3.Scale(shelfData.offset , new(-1,1,1));
        }
    }
}