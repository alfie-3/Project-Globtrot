using UnityEngine;

public class UI_HostSessionManager : MonoBehaviour
{
    [SerializeField] Canvas mainCanvas;
    [SerializeField] UI_LobbyManager lobby;

    public async void Host()
    {
        UI_MenusManager.SetDisplayLoadingScreen.Invoke(true);

        SessionManager.TaskResult result = await SessionManager.HostSession();

        if (result.Equals(SessionManager.TaskResult.Faliure))
        {
            Debug.Log("Hosting failed");
            UI_MenusManager.SetDisplayLoadingScreen.Invoke(false);

            return;
        }

        UI_MenusManager.SetDisplayLoadingScreen.Invoke(false);
        lobby.StartLobby();
    }
}
