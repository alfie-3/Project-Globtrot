using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class LocalNavmeshSurfaceBuilder : MonoBehaviour
{
    List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();

    NavMeshData navMeshData;
    NavMeshDataInstance navMeshDataInstance;

    public Vector3 SizeBounds = new Vector3(80, 20, 80);

    private void Awake()
    {
        NavMeshSourceTag.RebuildNavmesh += RebuildNavmeshSurface;
    }

    private void Start()
    {
        navMeshData = new NavMeshData();
    }

    public void RebuildNavmeshSurface()
    {
        NavMeshSourceTag.Collect(ref sources);

        var defaultBuildSettings = NavMesh.GetSettingsByID(0);
        var bounds = QuantizedBounds();

        navMeshData = NavMeshBuilder.BuildNavMeshData(defaultBuildSettings, sources, bounds, transform.position, transform.rotation);

        NavMeshBuilder.UpdateNavMeshDataAsync(navMeshData, defaultBuildSettings, sources, bounds);

        AddNavmeshData();
    }

    private void AddNavmeshData()
    {
        if (navMeshData != null)
        {
            if (navMeshDataInstance.valid)
            {
                NavMesh.RemoveNavMeshData(navMeshDataInstance);
            }
            navMeshDataInstance = NavMesh.AddNavMeshData(navMeshData);
        }
    }

    Bounds QuantizedBounds()
    {
        // Quantize the bounds to update only when theres a 10% change in size
        var center = transform.position;
        return new Bounds(Quantize(center, 0.1f * SizeBounds), SizeBounds);
    }

    static Vector3 Quantize(Vector3 v, Vector3 quant)
    {
        float x = quant.x * Mathf.Floor(v.x / quant.x);
        float y = quant.y * Mathf.Floor(v.y / quant.y);
        float z = quant.z * Mathf.Floor(v.z / quant.z);
        return new Vector3(x, y, z);
    }

    public void OnDrawGizmos()
    {
        if (navMeshData)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(navMeshData.position, navMeshData.sourceBounds.size);
        }

        Gizmos.color = Color.yellow;
        var bounds = QuantizedBounds();
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }

    private void OnDisable()
    {
        NavMeshSourceTag.RebuildNavmesh -= RebuildNavmeshSurface;

        if (navMeshData != null)
        {
            if (navMeshDataInstance.valid)
            {
                NavMesh.RemoveNavMeshData(navMeshDataInstance);
            }
        }
    }

    private void OnDestroy()
    {
        NavMeshSourceTag.RebuildNavmesh -= RebuildNavmeshSurface;
    }
}
