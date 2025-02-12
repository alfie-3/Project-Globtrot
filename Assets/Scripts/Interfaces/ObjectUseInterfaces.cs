using UnityEngine;

public interface IUsePrimary
{
    public void UsePrimary(PlayerHoldingManager manager);
}

public interface IUseSecondary
{
    public void UseSecondary(PlayerHoldingManager manager);
}

public interface IOnHeld
{
    public void OnHeld(PlayerHoldingManager manager);
}

public interface IOnDrop
{
    public void OnDrop( PlayerHoldingManager manager);
}

public interface IUpdate
{
    public void OnUpdate(PlayerHoldingManager manager);
}