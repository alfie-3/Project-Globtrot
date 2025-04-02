using UnityEngine;

public class RespawnZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerCharacterController controller))
        {
            controller.Respawn();
        }
    }
}
