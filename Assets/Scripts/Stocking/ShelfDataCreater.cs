using Unity.Mathematics;
using UnityEngine;

public class ShelfDataCreater : MonoBehaviour
{

    [SerializeField] public string ItemScriptableObjectName;
    [SerializeField] public Vector3 gizmoOffset;
    [SerializeField] public Vector3 offset;
    [SerializeField] public Vector3 objectBounds;
    [SerializeField] public Vector3 stackBounds;
    [SerializeField] public Vector3 EvenOutOffset;


    void OnDrawGizmosSelected()
    {

#if UNITY_EDITOR
        Gizmos.color = Color.red;

        //Draw the suspension
        Gizmos.DrawLine(
            transform.position,
            Vector3.up
        );

        for(int x = 0; x < stackBounds.x; x++)
        {
            for (int z = 0; z < stackBounds.z; z++)
            {
                for (int y = 0; y < stackBounds.y; y++)
                {
                    Gizmos.DrawWireCube(new Vector3((objectBounds.x) * x, (objectBounds.y) * y, (objectBounds.z) * z)+transform.position+gizmoOffset+EvenOutOffset,objectBounds);
                }
            }
        }

        //draw force application point
        Gizmos.DrawWireSphere(Vector3.zero, 0.05f);

        Gizmos.color = Color.white;
#endif
    }
}
