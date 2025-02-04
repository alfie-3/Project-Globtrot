using UnityEngine;

public class PlayerHoldingManager : MonoBehaviour
{
    [field: SerializeField] public ItemBase HeldItem { get; private set; }
    [SerializeField] public Material Material;
    public float Rotation { get; private set; }

    public PlayerCameraManager CameraManager { get; private set; }

    private void Awake()
    {
        PlayerInputManager playerInputManager = GetComponent<PlayerInputManager>();

        playerInputManager.OnPerformPrimary += PerformPrimary;
        playerInputManager.OnPerformSecondary += PerformSecondary;
        playerInputManager.OnRotate += PerformRotate;

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

    public void Update() {
        if (HeldItem != null) {
            ((Placable_Item)HeldItem).ShowMesh(this);
        }
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

    public void PerformRotate(float dir)
    {
        if (HeldItem == null) return;

        Rotation += dir * 8;
    }

    public void ClearItem()
    {
        HeldItem = null;
        Rotation = 0;
    }

    public void DropItem()
    {

    }
}
