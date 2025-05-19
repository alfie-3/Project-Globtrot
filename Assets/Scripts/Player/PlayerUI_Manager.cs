using Unity.Netcode;
using UnityEngine;

public class PlayerUI_Manager : NetworkBehaviour
{
    [SerializeField] UI_PlayerMenusManager PlayerUIPrefab;
    public UI_PlayerMenusManager PlayerUI {  get; private set; }

    [SerializeField] ThrowMeterAnim ThrowMeterAnim;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsLocalPlayer) return;

        PlayerUI = Instantiate(PlayerUIPrefab);

        ThrowMeterAnim = PlayerUI.GetComponentInChildren<ThrowMeterAnim>();
        PlayerUI.GetComponent<UI_PlayerMenusManager>().Init(this);

        ThrowMeterAnim.SetThrowMaxDuration(GetComponent<PlayerHoldingManager>().maxThrowForceChargeTime);
        GetComponent<PlayerHoldingManager>().Throwing += ThrowMeterAnim.ThrowingState;


        GetComponent<PlayerBuildingManager>().ui = PlayerUI.GetComponentInChildren<UI_BuildingSelection>();

        foreach (IInitPlayerUI init in PlayerUI.GetComponentsInChildren<IInitPlayerUI>())
        {
            init.Init(this);
        }
    }
}

public interface IInitPlayerUI
{
    public void Init(PlayerUI_Manager uiManager);
}