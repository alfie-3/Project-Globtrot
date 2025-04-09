using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UI_PlayerMenusManager : MonoBehaviour, IEscapeable
{
    PlayerInputManager connectedPlayerInput;

    [SerializeField] Canvas mainCanvas;
    [SerializeField] Canvas pauseCanvas;

    public void Init(PlayerUI_Manager player)
    {
        if (player.TryGetComponent(out PlayerInputManager inputManager))
        {
            connectedPlayerInput = inputManager;

            inputManager.EscapeStack = new();
            inputManager.EscapeStack.Push(this);
        }
    }

    public void Escape(PlayerInputManager manager)
    {
        TogglePauseMenu();
    }

    public void TogglePauseMenu()
    {
        bool pausedState = pauseCanvas.enabled;

        pauseCanvas.enabled = !pausedState;
        mainCanvas.enabled = pausedState;

        connectedPlayerInput.ToggleUIInput(!pausedState);
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
        if (SessionManager.Session != null)
        {
            await SessionManager.LeaveSession();
        }

        NetworkManager.Singleton.Shutdown();
    }
}
