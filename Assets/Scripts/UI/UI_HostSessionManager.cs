using UnityEngine;

public class UI_HostSessionManager : MonoBehaviour
{
    [SerializeField] Canvas mainCanvas;
    [SerializeField] UI_LobbyManager lobby;

    public async void Host()
    {
        UI_MenusManager.Singleton.DisplayLoadingScreen(true);

        SessionManager.TaskResult result = await SessionManager.HostSession();

        if (result.Equals(SessionManager.TaskResult.Faliure))
        {
            Debug.Log("Hosting failed");
            UI_MenusManager.Singleton.DisplayLoadingScreen(false);

            return;
        }

        UI_MenusManager.Singleton.DisplayLoadingScreen(false);
        lobby.StartLobby();
    }
}
