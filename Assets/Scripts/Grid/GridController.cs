using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GridController : MonoBehaviour
{
    Grid grid;
    public static GridController Instance { get; private set; }
    [field: SerializeField] public bool objectInMiddleOfCell { get; private set; }
    [field: SerializeField] public Vector3 gridOffset { get; private set; }


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

    public void SetVisabiltay(bool visibilaty)
    {
        if (transform.childCount == 0) return;
        transform.GetChild(0).GetComponent<Renderer>().enabled = visibilaty;
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
class CustomGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        DrawDefaultInspector();

        GridController grid = (GridController)target;

        if (GUILayout.Button("Set grid material"))
        {
            grid.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial.SetVector("_Offset", new Vector2(grid.gridOffset.x,grid.gridOffset.z));
            grid.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial.SetVector("_CellSize", grid.GetComponent<Grid>().cellSize);
        }
    }
}
#endif

 