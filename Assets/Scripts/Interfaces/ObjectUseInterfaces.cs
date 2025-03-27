using UnityEngine;
using UnityEngine.InputSystem;

public interface IUsePrimary
{
    public void UsePrimary(PlayerHoldingManager manager);
}

public interface IUseSecondary
{
    public void UseSecondary(PlayerHoldingManager manager);
}

public interface IUseItem
{
    public void OnItemUsed(PlayerHoldingManager holdingManager, Stock_Item shopProduct_Item);
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