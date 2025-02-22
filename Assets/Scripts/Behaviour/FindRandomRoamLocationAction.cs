using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FindRandomRoamLocation", story: "Find random roam [location]", category: "Action/Find", id: "796ada47b9b3b3d5ba58d9a982370f28")]
public partial class FindRandomRoamLocationAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Location;

    protected override Status OnStart()
    {
        GameObject roamLocation = RoamLocation.GetRandomRoamLocation().gameObject;

        if (roamLocation == null) return Status.Failure;

        Location.Value = roamLocation;
        return Status.Success;
    }
}

