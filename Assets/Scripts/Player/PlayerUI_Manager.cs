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

        GetComponent<PlayerHoldingManager>().Throwing += UpdateThrowingUI;
    }

    void UpdateThrowingUI(bool val)
    {
        if (val)
            ThrowMeterAnim.StartAnim(GetComponent<PlayerHoldingManager>().maxThrowForceChargeTime);
        else
            ThrowMeterAnim.Thrown();
    }
}
