using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RequiredBuildablesManager : NetworkBehaviour
{
    public List<PlacableFurniture_Item> RequiredBuildables = new();

    public Dictionary<PlacableFurniture_Item, int> CurrentBuildables = new();

    public static RequiredBuildablesManager Instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        Instance = null;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void AddRequiredBuildable(PlacableFurniture_Item placeable)
    {
        if (Instance == null) return;

        Instance.RequiredBuildables.Add(placeable);
    }

    public static void AddBuildable(PlacableFurniture_Item placable)
    {
        if (Instance == null) return;

        if (Instance.CurrentBuildables.ContainsKey(placable))
        {
            Instance.CurrentBuildables[placable]++;
        }
        else
        {
            Instance.CurrentBuildables.Add(placable, 1);
        }
    }

    public static void RemoveBuildable(PlacableFurniture_Item furnitureItem)
    {
        if (Instance == null) return;

        if (Instance.CurrentBuildables.ContainsKey(furnitureItem))
        {
            if (Instance.CurrentBuildables[furnitureItem] - 1 <= 0)
            {
                Instance.CurrentBuildables.Remove(furnitureItem);
            }
            else
            {
                Instance.CurrentBuildables[furnitureItem]--;
            }
        }
    }

    public static bool HasRequiredBuildables(out RequiredBuildablesResponse response)
    {
        if (Instance == null)
        {
            response = default;
            return false;
        }

        HashSet<PlacableFurniture_Item> missingBuildables = new();

        foreach (PlacableFurniture_Item reqBuildable in Instance.RequiredBuildables)
        {
            if (Instance.CurrentBuildables.ContainsKey(reqBuildable))
            {
                continue;
            }

            missingBuildables.Add(reqBuildable);
        }

        if (missingBuildables.Count > 0)
        {
            response = new(missingBuildables);
            return false;
        }
        else
        {
            response = default;
            return true;
        }
    }
}

public struct RequiredBuildablesResponse
{
    public HashSet<PlacableFurniture_Item> MissingBuildables;

    public RequiredBuildablesResponse(HashSet<PlacableFurniture_Item> missingBuildables)
    {
        MissingBuildables = missingBuildables;
    }

    public string CreateNotification()
    {
        string message = "MISSING BUILDABLES:";

        foreach (PlacableFurniture_Item item in MissingBuildables)
        {
            message += ($"<br> {item.ItemName}");
        }

        return message;
    }
}