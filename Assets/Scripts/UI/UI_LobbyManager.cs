using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_LobbyManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI lobbyIdText;
    [Space]
    [SerializeField] Transform playerCardHolder;
    [SerializeField] UI_PlayerCard playerCardPrefab;

    Dictionary<string, UI_PlayerCard> playerCardDict = new Dictionary<string, UI_PlayerCard>();

    [SerializeField] Button startGameButton;

    private void OnEnable()
    {
        SessionManager.PlayerJoined += AddPlayerCard;
        SessionManager.PlayerLeft += RemovePlayerCard;

        GetComponent<UI_OpenableCanvas>().CanvasOpened += ToggleStartButton;
    }

    public void StartLobby()
    {
        GetComponent<UI_OpenableCanvas>().SetEnabled(true);
        lobbyIdText.text = SessionManager.Session.Code;

        AddPlayerCards();
    }

    public async void LeaveLobby()
    {
        await SessionManager.LeaveSession();
        GetComponent<UI_OpenableCanvas>().SetEnabled(false);
        ClearPlayerCards();
    }

    public void StartGame(string sceneName)
    {
        SessionManager.LoadScene(sceneName);
    }

    public void AddPlayerCards()
    {
        foreach (IReadOnlyPlayer player in SessionManager.Session.Players)
        {
            AddPlayerCard(player.Id);
        }
    }

    public void AddPlayerCard(string playerId)
    {
        foreach (IReadOnlyPlayer player in SessionManager.Session.Players)
        {
            if (player.Id == playerId)
            {
                UI_PlayerCard card = Instantiate(playerCardPrefab.gameObject, playerCardHolder).GetComponent<UI_PlayerCard>();
                playerCardDict.Add(player.Id, card);
                card.InitCard(player);
            }
        }
    }

    public void RemovePlayerCard(string playerId)
    {
        if (playerCardDict.TryGetValue(playerId, out var card))
        {
            Destroy(card.gameObject);
        }

        playerCardDict.Remove(playerId);
    }

    public void ClearPlayerCards()
    {
        foreach (Transform child in playerCardHolder.transform)
        {
            Destroy(child.gameObject);
        }

        playerCardDict.Clear();
    }

    public void ToggleStartButton(UI_OpenableCanvas _)
    {
        startGameButton.interactable = SessionManager.Session.IsHost;
    }
}
