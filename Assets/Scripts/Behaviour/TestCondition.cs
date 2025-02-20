using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Test", story: "Terst", category: "Conditions", id: "74b5a467a727a8cebc76a88b44d4aa08")]
public partial class TestCondition : Condition
{

    public override bool IsTrue()
    {
        return false;
    }
}
