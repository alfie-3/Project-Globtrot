using Unity.Netcode;
using UnityEngine;

public class MessMaker : NetworkBehaviour
{
    [SerializeField] MessController mess;
    [SerializeField] LayerMask layerMask;

    [SerializeField] float randomMessChance = 1;

    public void CreateMess()
    {
        if (Random.value > randomMessChance) return;

        Debug.DrawRay(transform.position, Vector3.down * 1, Color.red, 3);
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1, layerMask))
        {
            if (!hit.transform.TryGetComponent(out MessSurface surface)) return;

            Vector3 euler = new(90, Random.Range(0, 360),0);
            MessController instancedMessController = Instantiate(mess, hit.point + mess.transform.position, Quaternion.Euler(euler));
            instancedMessController.GetComponent<NetworkObject>().Spawn();
        }
    }
}
