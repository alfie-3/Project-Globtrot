using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UI_LobbyManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI lobbyIdText;

    public void StartLobby()
    {
        GetComponent<UI_OpenableCanvas>().SetEnabled(true);
        lobbyIdText.text = SessionManager.Session.Code;
    }

    public async void LeaveLobby()
    {
        await SessionManager.LeaveSession();
        GetComponent<UI_OpenableCanvas>().SetEnabled(false);
    }
}
