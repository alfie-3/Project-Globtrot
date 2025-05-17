using System;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IUsePrimary
{
    public void UsePrimary(PlayerHoldingManager manager);
    public InteractionContext GetUseContext(PlayerHoldingManager manager);
}

public interface IUseSecondary
{
    public void UseSecondary(PlayerHoldingManager manager);
}

public interface IUseItem
{
    public bool CanUseItem(PlayerHoldingManager holdingManager, Stock_Item item);
    public void OnItemUsed(PlayerHoldingManager holdingManager, Stock_Item shopProduct_Item);

    public InteractionContext OnViewWithItem(PlayerHoldingManager holdingManager, Stock_Item item);
    public void OnUnview();
}

public struct InteractionContext : IEquatable<InteractionContext>
{
    public bool InteractionAvailable;
    public string InteractionContextText;

    public InteractionContext(bool canInteract, string interactionContextText = "Interact")
    {
        InteractionAvailable = canInteract;
        InteractionContextText = interactionContextText;
    }

    public static InteractionContext DefaultContext => new InteractionContext(true, "Interact");

    public bool Equals(InteractionContext other) => InteractionAvailable == other.InteractionAvailable && InteractionContextText.Equals(other.InteractionContextText);
}

public interface IOnHeld
{
    public void OnHeld(PlayerHoldingManager manager);
}

public interface IOnDrop
{
    public void OnDrop( PlayerHoldingManager manager);
}

public interface IScroll
{
    public void OnScroll(PlayerHoldingManager manager, InputAction.CallbackContext context);
}

public interface IUpdate
{
    public void OnUpdate(PlayerHoldingManager manager);
}

public interface IOnCtrl 
{
    public void OnCtrl(PlayerHoldingManager manager);
}