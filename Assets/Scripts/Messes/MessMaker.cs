using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class MessMaker : NetworkBehaviour
{
    [SerializeField] MessController mess;
    [SerializeField] ParticleSystem particles;
    [SerializeField] LayerMask layerMask;

    [SerializeField] float randomMessChance = 1;

    public void CreateMess()
    {
        if (!IsServer) return;

        if (Random.value > randomMessChance) return;

        Debug.DrawRay(transform.position, Vector3.down * 1, Color.red, 3);
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1, layerMask))
        {
            if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 2, NavMesh.GetAreaFromName("Everything")))
            {
                Vector3 euler = new(mess.Decal ? 90 : 0, Random.Range(0, 360), 0);
                MessController instancedMessController = Instantiate(mess, navHit.position + mess.transform.position, Quaternion.Euler(euler));
                instancedMessController.GetComponent<NetworkObject>().Spawn();

                if (particles)
                {
                    PlayParticles_Rpc(hit.point);
                }
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    public void PlayParticles_Rpc(Vector3 position)
    {
        Instantiate(particles, position, Quaternion.identity);
    }
}
