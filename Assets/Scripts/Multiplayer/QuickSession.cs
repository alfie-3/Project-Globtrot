using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.Events;
using static SessionManager;

public class QuickSession : MonoBehaviour
{
    public static ISession Session { get; private set; }
    public static ConnectionState State { get; private set; } = ConnectionState.Disconnected;

    public UnityEvent OnConnect;
    public UnityEvent OnDisconnect;

    public async void HostOrJoinSession()
    {
        //Signs in the player, returns out if sign in fails
        TaskResult playerSignInResult = await SignInPlayer();

        if (playerSignInResult.Equals(TaskResult.Faliure))
        {
            Debug.Log("Player sign in error");
        }

        //Generates a random session ID for simplicity
        string SessionName = UnityEngine.Random.Range(10, 99).ToString();

        var playerProperties = PlayerProperties.GetPlayerProperties();

        // Set the session options.
        var options = new SessionOptions()
        {
            Name = SessionName,
            MaxPlayers = MAXPLAYERS,
            PlayerProperties = playerProperties
        };

        Debug.Log($"Creating session {options.Name}...");

        //Attempts to host new session
        try
        {
            Session = await MultiplayerService.Instance.CreateOrJoinSessionAsync("1", options);
            State = ConnectionState.Connected;
            OnConnect?.Invoke();
        }
        catch (Exception e)
        {
            State = ConnectionState.Disconnected;
            Debug.Log(e);
            OnDisconnect?.Invoke();

            return;
        }

        TrySpawn();
    }

    public void TrySpawn()
    {
        GameObject foundGO = GameObject.FindGameObjectWithTag("SpawnManager");

        if (foundGO == null) return;

        if (foundGO.TryGetComponent(out SpawnManager spawnManager))
        {
            spawnManager.RequestSpawnPlayer_Rpc(NetworkManager.Singleton.LocalClientId, PlayerProfile.CharacterReferenceData.PlayerReferenceID);
        }
    }
}
