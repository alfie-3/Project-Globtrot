using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class ClugManager : NetworkBehaviour
{
    public static ClugManager Instance { get; private set; }

    
    protected List<Clug> ClugList { get; private set; } = new List<Clug>();

    private RandomPointOnMesh rndPoint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);

        rndPoint = GetComponent<RandomPointOnMesh>();

        InvokeRepeating("CheckClugs", 1,7.5f);
    }




    void CheckClugs()
    {
        foreach(Clug clug in ClugList)
        {
            if (Vector3.Distance(clug.transform.position, clug.GetLastPos()) < 0.2f)
            {
                SendClugToRndPos(clug);
                
            }
        }
    }


    void SendClugToRndPos(Clug clug)
    {
        NavMeshTriangulation triangles = NavMesh.CalculateTriangulation();
        Mesh mesh = new Mesh();
        mesh.vertices = triangles.vertices;
        mesh.triangles = triangles.indices;
        Vector3 pos = rndPoint.GetRandomPointOnMesh(mesh);
        clug.Target = pos;
        clug.Invoke("GoToTarget", Random.Range(0f, 5f));
    }

    public void AddClug(Clug clug)
    {
        ClugList.Add(clug);
        SendClugToRndPos(clug);
    }
    public void RemoveClug(Clug clug)
    {
        ClugList.Remove(clug);
    }

}
