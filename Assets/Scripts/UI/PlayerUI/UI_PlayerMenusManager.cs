using System;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UI_PlayerMenusManager : MonoBehaviour, IEscapeable
{
    PlayerInputManager connectedPlayerInput;

    [SerializeField] Canvas UICanvas;
    [SerializeField] Canvas mainCanvas;
    [SerializeField] Canvas pauseCanvas;

    bool enablePaueMenu = true;

    private void OnEnable()
    {
        GameStateManager.OnDayStateChanged += UpdateUIVisible;
    }

    private void UpdateUIVisible(DayState state)
    {
        enablePaueMenu = state != DayState.DayEnd;
        SetPlayerUIEnabled(state != DayState.DayEnd);
    }

    public void Init(PlayerUI_Manager player)
    {
        if (player.TryGetComponent(out PlayerInputManager inputManager))
        {
            connectedPlayerInput = inputManager;

            inputManager.EscapeList = new()
            {
                this
            };
        }
    }

    public void Escape(PlayerInputManager manager)
    {
        TogglePauseMenu();
    }

    public void TogglePauseMenu()
    {
        if (!enablePaueMenu) return;

        bool pausedState = pauseCanvas.enabled;

        pauseCanvas.enabled = !pausedState;
        mainCanvas.enabled = pausedState;

        connectedPlayerInput.SetUIInput(!pausedState);
        connectedPlayerInput.CameraManager.SetPanTiltEnabled(pausedState);

        if (!pausedState)
        {
            CursorUtils.UnlockAndShowCursor();
        }
        else
        {
            CursorUtils.LockAndHideCusor();
        }
    }

    public void SetPlayerUIEnabled(bool value)
    {
        UICanvas.enabled = value;
    }


    public async void LeaveSession()
    {

        await ShutdownNetworking();
        SceneManager.LoadScene(0);
    }

    public async void QuitGame()
    {
        await ShutdownNetworking();
        Application.Quit();
    }

    public async Task ShutdownNetworking()
    {
        LobbyStateManager.ForceDisconnectClients();

        if (SessionManager.Session != null)
        {
            await SessionManager.LeaveSession();
        }

        NetworkManager.Singleton.Shutdown();
    }

    private void OnDestroy()
    {
        GameStateManager.OnDayStateChanged -= UpdateUIVisible;

    }
}
