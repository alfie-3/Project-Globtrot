using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class MessMaker : NetworkBehaviour
{
    [SerializeField] MessListSO messList;
    [SerializeField] int randomChance = 1000;
    [Space]
    [SerializeField] float messRadius = 3;

    private void FixedUpdate()
    {
        if (!IsServer) return;

        CheckCreateMess();
    }

    public void CheckCreateMess()
    {
        if (Random.Range(0, randomChance) == 0)
        {
            CreateMess();
        }
    }

    public void CreateMess()
    {
        if (messList == null) return; 

        MessController messController = messList.GetMessRandom();

        Vector3 randomTarget = transform.position + (Vector3)(messRadius * Random.insideUnitCircle);

        if (NavMesh.SamplePosition(randomTarget, out NavMeshHit hit, messRadius, NavMesh.GetAreaFromName("Everything")))
        {
            MessController instancedMessController = Instantiate(messController, hit.position + messController.transform.position, messController.transform.rotation);
            instancedMessController.GetComponent<NetworkObject>().Spawn();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, messRadius);
    }
}
