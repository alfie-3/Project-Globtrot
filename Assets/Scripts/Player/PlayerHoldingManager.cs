using UnityEngine;

public class PlayerHoldingManager : MonoBehaviour
{
    [field: SerializeField] public ItemBase HeldItem { get; private set; }

    public PlayerCameraManager CameraManager { get; private set; }

    private void Awake()
    {
        PlayerInputManager playerInputManager = GetComponent<PlayerInputManager>();

        playerInputManager.OnPerformPrimary += PerformPrimary;
        playerInputManager.OnPerformSecondary += PerformSecondary;

        CameraManager = GetComponentInChildren<PlayerCameraManager>();
    }

    public void HoldItem(ItemBase item)
    {
        if (item == null)
        {
            Debug.LogWarning("No referenced item - Did you forget to assign an item?");
            return;
        }

        HeldItem = item;
        HeldItem.OnHeld();
    }

    public void PerformPrimary()
    {
        if (HeldItem == null) return;

        HeldItem.OnPrimary(this);
    }

    public void PerformSecondary()
    {
        if (HeldItem == null) return;

        HeldItem.OnSecondary(this);
    }

    public void DropItem()
    {

    }
}
