using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SessionManager
{
    public static ISession Session { get; private set; }

    public const int MAXPLAYERS = 5;
    public static int CurrentPlayers;

    public static ConnectionState State { get; private set; } = ConnectionState.Disconnected;

    public static Action<string> PlayerJoined;
    public static Action<string> PlayerLeft;


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        Session = null;

        CurrentPlayers = 0;

        PlayerJoined = delegate { };
        PlayerLeft = delegate { };
        State = ConnectionState.Disconnected;
    }

    public enum ConnectionState
    {
        Disconnected,
        Connecting,
        Connected,
    }

    public static void RegisterSessionEvents()
    {
        Session.PlayerJoined += (value) => { PlayerJoined.Invoke(value); };
        Session.PlayerHasLeft += (value) => { PlayerLeft.Invoke(value); };

        NetworkManager.Singleton.OnClientConnectedCallback += context => { Debug.Log($"Client {context} connected"); };
    }

    public static void UnregisterSessionEvents()
    {
        Session.PlayerJoined -= (value) => { PlayerJoined.Invoke(value); };
        Session.PlayerHasLeft -= (value) => { PlayerLeft.Invoke(value); };
    }

    public static async Task<TaskResult> HostSession()
    {
        if (State == ConnectionState.Connected)
        {
            Debug.Log("Already connected");
            return TaskResult.Faliure;
        }

        //Signs in the player, returns out if sign in fails
        TaskResult playerSignInResult = await SignInPlayer();

        if (playerSignInResult.Equals(TaskResult.Faliure))
        {
            Debug.Log("Player sign in error");
            return TaskResult.Faliure;
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
        }.WithRelayNetwork();

        Debug.Log($"Creating session {options.Name}...");

        //Attempts to host new session
        try
        {
            Session = await MultiplayerService.Instance.CreateSessionAsync(options);
            State = ConnectionState.Connected;
        }
        catch (Exception e)
        {
            State = ConnectionState.Disconnected;
            Debug.Log(e);
            return TaskResult.Faliure;
        }

        CurrentPlayers = 0;
        RegisterSessionEvents();

        return TaskResult.Success;
    }

    public static async Task<TaskResult> JoinSession(string sessionCode)
    {
        if (State == ConnectionState.Connected) return TaskResult.Faliure;

        //Signs in the player, returns out if sign in fails
        TaskResult result = await SignInPlayer();

        if (result.Equals(TaskResult.Faliure))
        {
            Debug.Log("Player sign in error");
            return TaskResult.Faliure;
        }

        Debug.Log($"Connecting to {sessionCode}...");

        //Sets player properties
        JoinSessionOptions options = new JoinSessionOptions()
        {
            PlayerProperties = PlayerProperties.GetPlayerProperties()
        };

        //Attempt connection to session via code
        try
        {
            Session = await MultiplayerService.Instance.JoinSessionByCodeAsync(sessionCode, options);
            State = ConnectionState.Connected;
        }
        catch (Exception e)
        {
            State = ConnectionState.Disconnected;
            Debug.Log(e);
            return TaskResult.Faliure;
        }

        RegisterSessionEvents();

        return TaskResult.Success;
    }

    public static void LoadScene(string sceneName)
    {
        if (!Session.IsHost) return;

        NetworkManager.Singleton.SceneManager.ActiveSceneSynchronizationEnabled = true;
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public static async Task LeaveSession()
    {
        if (Session != null)
        {
            await Session.LeaveAsync();
        }

        UnregisterSessionEvents();

        State = ConnectionState.Disconnected;
    }

    public static async Task<TaskResult> SignInPlayer()
    {
        if (string.IsNullOrEmpty(PlayerProfile.PlayerName))
        {
            PlayerProfile.SetPlayerName("3i29132");
        }

        try
        {
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                AuthenticationService.Instance.SwitchProfile(PlayerProfile.PlayerName);
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                await AuthenticationService.Instance.UpdatePlayerNameAsync(PlayerProfile.PlayerName);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return TaskResult.Faliure;
        }

        return TaskResult.Success;
    }

    public enum TaskResult
    {
        Success,
        Faliure
    }
}
