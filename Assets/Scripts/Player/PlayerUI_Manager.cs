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

        Canvas UI = Instantiate(PlayerUI);

        ThrowMeterAnim = UI.GetComponentInChildren<ThrowMeterAnim>();
        UI.GetComponent<UI_PlayerMenusManager>().Init(this);

        ThrowMeterAnim.SetThrowMaxDuration(GetComponent<PlayerHoldingManager>().maxThrowForceChargeTime);
        GetComponent<PlayerHoldingManager>().Throwing += ThrowMeterAnim.ThrowingState;


        GetComponent<PlayerBuildingManager>().ui = UI.GetComponentInChildren<UI_BuildingSelection>();
    }

}
