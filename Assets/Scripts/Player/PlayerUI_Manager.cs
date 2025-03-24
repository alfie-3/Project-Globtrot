using Unity.Netcode;
using UnityEngine;

public class PlayerUI_Manager : NetworkBehaviour
{
    [SerializeField] Canvas PlayerUI;
    [SerializeField] ThrowMeterAnim ThrowMeterAnim;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsLocalPlayer) return;

        ThrowMeterAnim = Instantiate(PlayerUI).GetComponentInChildren<ThrowMeterAnim>();

        ThrowMeterAnim.SetThrowMaxDuration(GetComponent<PlayerHoldingManager>().maxThrowForceChargeTime);
        GetComponent<PlayerHoldingManager>().Throwing += ThrowMeterAnim.ThrowingState;
    }

}
