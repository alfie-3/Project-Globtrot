using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GridController : MonoBehaviour
{
    Grid grid;
    public static GridController Instance { get; private set; }
    [SerializeField] bool objectInMiddleOfCell;
    [SerializeField] Vector3 gridOffset;
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        grid = GetComponent<Grid>(); 
    }
    [SerializeField] private Vector2 GizmoVec;

    public Vector3 HitToGrid(Vector3 hitPosition)
    {
        Vector3 position = hitPosition + gridOffset;// + (grid.cellSize * 0.5f);
        Debug.DrawLine(position,position+Vector3.up);
        Vector3 gridPos = grid.WorldToCell(position);
        position = Vector3.Scale(gridPos, grid.cellSize) + (objectInMiddleOfCell ? (grid.cellSize * 0.5f) : Vector3.zero);
        position = new Vector3(position.x, hitPosition.y, position.z);
        return position - gridOffset;
    }

    public void SetVisabiltay(bool visibilaty, ulong clientID)
    {

        SetVisibilaty_RPC(visibilaty, clientID,transform.GetChild(0).GetComponent<NetworkObject>());
    }
    [Rpc(SendTo.Server)]
    private void SetVisibilaty_RPC(bool visibilaty, ulong clientID, NetworkObjectReference gridReference)
    {
        gridReference.TryGet(out NetworkObject @object);
        if (visibilaty)
            @object.NetworkShow(clientID); 
        else
            @object.NetworkHide(clientID);
    }


void OnDrawGizmosSelected()
    {

#if UNITY_EDITOR
        Gizmos.color = Color.blue;

        //Draw the suspension
        Gizmos.DrawLine(
            transform.position,
            Vector3.up
        );
        Vector3 pos = Vector3.zero;
        for (pos.x = 0; pos.x < GizmoVec.x; pos.x++)
        {
            for (pos.z = 0; pos.z < GizmoVec.y; pos.z++)
            {
                Gizmos.DrawWireCube(Vector3.Scale(pos , GetComponent<Grid>().cellSize) + Vector3.Scale(pos , GetComponent<Grid>().cellGap) + transform.position + (GetComponent<Grid>().cellSize*0.5f) - (gridOffset), 
                    GetComponent<Grid>().cellSize);
            }
        }
        Gizmos.color = Color.green;
        //Gizmos.DrawWireCube(Vector3.Scale(objectBounds, positionTest) + transform.position + gizmoOffset, objectBounds);

        //draw force application point
        Gizmos.DrawWireSphere(Vector3.zero, 0.05f);
        Gizmos.color = Color.white;
#endif
    }
}


#if UNITY_EDITOR

[CustomEditor(typeof(GridController))]
class CustomGridyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        DrawDefaultInspector();

        GridController gridy = (GridController)target;

        if (GUILayout.Button("setGrid"))
        {
        }
    }
}
#endif