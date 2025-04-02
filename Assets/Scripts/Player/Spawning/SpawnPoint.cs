using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] Color gizmoColour;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColour;
        Gizmos.DrawCube(transform.position + (transform.up * 1.8f /2 ), new(0.5f, 1.8f, 0.5f));
        Gizmos.DrawRay(new(transform.position + (transform.up * 1.5f), transform.forward));
    }
}
