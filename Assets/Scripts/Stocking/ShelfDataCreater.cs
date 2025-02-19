using Unity.Mathematics;
using UnityEngine;

public class ShelfDataCreater : MonoBehaviour
{

    public string itemId;
    [SerializeField] public Vector3 gizmoOffset;
    [SerializeField] public Vector3 offset;
    [SerializeField] public Vector3 objectBounds;
    [SerializeField] public Vector3 stackBounds;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



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
                    Gizmos.DrawWireCube(new Vector3((objectBounds.x * -1) * x, (objectBounds.y * 1) * y, (objectBounds.z * 1) * z)+transform.position+gizmoOffset,objectBounds);
                }
            }
        }

        //draw force application point
        Gizmos.DrawWireSphere(Vector3.zero, 0.05f);

        Gizmos.color = Color.white;
#endif
    }
}
