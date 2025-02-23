using System.Collections.Generic;
using UnityEngine;

public class RoamLocation : MonoBehaviour
{
    public static List<RoamLocation> RoamLocations = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        RoamLocations = new();
    }

    public void OnEnable()
    {
        RoamLocations.Add(this);
    }

    private void OnDisable()
    {
        RoamLocations.Remove(this);
    }

    public static RoamLocation GetRandomRoamLocation()
    {
        if (RoamLocations.Count == 0) { return null; }

        return RoamLocations[Random.Range(0, RoamLocations.Count)];
    }
}
