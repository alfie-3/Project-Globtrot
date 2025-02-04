using UnityEngine;

public abstract class ItemBase : ScriptableObject
{
    [field: SerializeField] public string ItemID {  get; protected set; }

    public virtual void OnUpdate(PlayerHoldingManager manager) { }
    public virtual void OnPrimary(PlayerHoldingManager holdingManager) { Debug.Log("Perform Primary"); }
    public virtual void OnSecondary(PlayerHoldingManager holdingManager) { Debug.Log("Perform Secondary"); }

    public virtual void OnHeld() { Debug.Log("Held"); }

}
