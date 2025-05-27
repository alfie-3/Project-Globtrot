using Unity.Netcode;
using UnityEngine;

public class DispenserController : NetworkBehaviour
{
    [SerializeField] Animator dispenserAnimator;

    [SerializeField] bool startDisabled;

    public override void OnNetworkSpawn()
    {
        SetDispenserDisabled(startDisabled);
    }

    public void SetDispenserDisabled(bool value)
    {
        dispenserAnimator.SetBool("Disabled", value);
    }
}
