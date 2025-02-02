using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] Color gizmoColour;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColour;
        Gizmos.DrawCube(transform.position, new(0.5f, 1.8f, 0.5f));
    }
}
