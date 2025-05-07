using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder;

public class NavMeshSourceTag : MonoBehaviour
{
    [SerializeField] int areaOverride = 0;

    public static List<NavmeshMeshFilterData> Mesheses = new();
    public static List<NavMeshModifierVolume> Modifiers = new();

    public static Action RebuildNavmesh = delegate { };

    NavmeshMeshFilterData meshFilterData;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        Mesheses = new List<NavmeshMeshFilterData>();
        Modifiers = new List<NavMeshModifierVolume>();

        RebuildNavmesh = () => { };
    }

    public class NavmeshMeshFilterData
    {
        public MeshFilter MeshFilter;
        public int Area;

        public NavmeshMeshFilterData(MeshFilter mesh, int area)
        {
            Area = area;
            MeshFilter = mesh;
        }
    }

    private void Awake()
    {
        if (TryGetComponent(out MeshFilter meshFilter))
        {
            meshFilterData = new(meshFilter, areaOverride);
            Mesheses.Add(meshFilterData);
        }

        if (TryGetComponent(out NavMeshModifierVolume meshModifier))
        {
            Modifiers.Add(meshModifier);    
        }
    }

    public void OnEnable()
    {

        RebuildNavmesh.Invoke();
    }

    public void OnDisable()
    {

        if (meshFilterData != null)
        {
            Mesheses.Remove(meshFilterData);
        }

        if (TryGetComponent(out NavMeshModifierVolume meshModifier))
        {
            Modifiers.Remove(meshModifier);
        }

        RebuildNavmesh.Invoke();
    }

    public static void Collect(ref List<NavMeshBuildSource> sources)
    {
        sources.Clear();

        for (int i = 0; i < Mesheses.Count; i++)
        {
            NavmeshMeshFilterData navmeshMeshFilterData = Mesheses[i];

            if (navmeshMeshFilterData.MeshFilter == null) continue;
            if (navmeshMeshFilterData.MeshFilter.sharedMesh == null) continue;

            NavMeshBuildSource buildSource = new NavMeshBuildSource();
            buildSource.shape = NavMeshBuildSourceShape.Mesh;
            
            buildSource.sourceObject = navmeshMeshFilterData.MeshFilter.sharedMesh;
            buildSource.transform = navmeshMeshFilterData.MeshFilter.transform.localToWorldMatrix;
            buildSource.area = navmeshMeshFilterData.Area;
            sources.Add(buildSource);
        }

        for (int i = 0; i < Modifiers.Count; i++)
        {
            NavMeshModifierVolume navModifier = Modifiers[i];

            if (navModifier == null) continue;

            NavMeshBuildSource modifierBuildSource = new NavMeshBuildSource();
            modifierBuildSource.shape = NavMeshBuildSourceShape.ModifierBox;

            modifierBuildSource.size = navModifier.size;

            Matrix4x4 baseMatrix = navModifier.transform.localToWorldMatrix;
            Matrix4x4 localMatrix = Matrix4x4.TRS(navModifier.center, Quaternion.identity, Vector3.one);    

            modifierBuildSource.transform = baseMatrix * localMatrix;

            modifierBuildSource.area = navModifier.area;

            sources.Add(modifierBuildSource);
        }

    }
}
