using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class UI_JoinSessionManager : MonoBehaviour
{
    [SerializeField] TMP_InputField sessionIdInputField;
    [SerializeField] UI_LobbyManager Lobby;

    public async void Join()
    {
        if (string.IsNullOrEmpty(sessionIdInputField.text))
        {
            Debug.Log("Invalid session ID");
            return;
        }

        UI_MenusManager.SetDisplayLoadingScreen.Invoke(true);

        SessionManager.TaskResult result = await SessionManager.JoinSession(sessionIdInputField.text);

        if (result.Equals(SessionManager.TaskResult.Faliure))
        {
            Debug.Log("Joining lobby error");
            UI_MenusManager.SetDisplayLoadingScreen.Invoke(false);

            return;
        }

        UI_MenusManager.SetDisplayLoadingScreen.Invoke(false);
        Lobby.StartLobby();
    }
}
